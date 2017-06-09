using PITCSurveyEntities.Entities;
using PITCSurveyLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PITCSurveySvc.Models
{
    /// <summary>
    /// Converts between JSON models and EF database entities.
    /// Entity-to-Model conversion is static, since EF lazy-load will provide the object graph needed.
    /// Model-to-Entity conversion is instance and needs DbContext, so it can pull mapping data from the db.
    /// </summary>
    public class ModelConverter
    {
        private PITCSurveyContext _db;
        
        public ModelConverter(PITCSurveyContext db)
        {
            _db = db;
        }

        #region "Entity-to-Model Conversion (Static)"

        public static SurveyModel ConvertToModel(Survey survey)
        {
            SurveyModel model = new SurveyModel()
            {
                SurveyID = survey.ID,
                Name = survey.Name,
                Description = survey.Description,
                IntroText = survey.IntroText,
                Version = survey.Version,
                LastUpdated = survey.LastUpdated
            };

            foreach (SurveyQuestion sq in survey.SurveyQuestions)
            {
                SurveyQuestionModel qm = new SurveyQuestionModel()
                {
                    QuestionID = sq.Question_ID,
                    QuestionNum = sq.QuestionNum,
                    QuestionText = sq.Question.QuestionText,
                    QuestionHelpText = sq.Question.ClarificationText,
                    AllowMultipleAnswers = sq.Question.AllowMultipleAnswers,
                    WellKnownQuestion = sq.Question.WellKnownQuestion
                };

                foreach (SurveyAnswerChoice ac in sq.AnswerChoices)
                {
                    SurveyQuestionAnswerChoiceModel acm = new SurveyQuestionAnswerChoiceModel()
                    {
                        AnswerChoiceID = ac.AnswerChoice_ID,
                        AnswerChoiceNum = ac.AnswerChoiceNum,
                        AnswerChoiceText = ac.AnswerChoice.AnswerText,
                        AdditionalAnswerDataFormat = ac.AnswerChoice.AdditionalAnswerDataFormat,
                        NextQuestionID = (ac.EndSurvey) ? SurveyQuestionAnswerChoiceModel.END_SURVEY : ac.NextSurveyQuestion?.Question_ID,
                        EndSurvey = ac.EndSurvey
                    };

                    qm.AnswerChoices.Add(acm);
                }

                model.Questions.Add(qm);
            }
            
            return model;
        }

        public static VolunteerModel ConvertToModel(Volunteer Volunteer)
        {
            VolunteerModel model = new VolunteerModel()
            {
                FirstName = Volunteer.FirstName,
                LastName = Volunteer.LastName,
                Email = Volunteer.Email,
                HomePhone = Volunteer.HomePhone,
                MobilePhone = Volunteer.MobilePhone
            };

            model.Address.Street = Volunteer.Address.Street;
            model.Address.City = Volunteer.Address.City;
            model.Address.State = Volunteer.Address.State;
            model.Address.ZipCode = Volunteer.Address.ZipCode;

            return model;
        }

        #endregion

        #region "Model-to-Entity Conversion"

        // These methods are not static, as they need the DbContext to pull in data for mapping.

        public Survey ConvertToEntity(SurveyModel model)
        {
            Survey survey = _db.Surveys.Where(s => s.Description == model.Description).SingleOrDefault();

            if (survey == null)
            {
                survey = new Survey()
                {
                    SurveyQuestions = new List<SurveyQuestion>()
                };

                _db.Surveys.Add(survey);
            }

            // For now, ignore ID - assume is new. We can delete existing if re-importing.
            survey.Name = model.Name;
            survey.Description = model.Description;
            survey.IntroText = model.IntroText;

            survey.LastUpdated = DateTimeOffset.Now;
            survey.Version += 1;

            // Map the provided IDs to the existing or db-generated questions, to preserve navigation mapping
            Dictionary<int, Question> questionsByModelID = new Dictionary<int, Question>();
            Dictionary<int, AnswerChoice> answerChoicesByModelID = new Dictionary<int, AnswerChoice>();

            // Process all questions and answer choices first, so we have them in the db and indexed by model ID.

            foreach (SurveyQuestionModel qm in model.Questions)
            {
                // See if question already exists. Remember, questions are reusable, and can be shared across surveys.

                Question q = _db.Questions.WhereEx(eq => eq.QuestionText == qm.QuestionText).SingleOrDefault();
                
                if (q == null)
                {
                    q = new Question();

                    _db.Questions.Add(q);

                    Trace.WriteLine($"+ Q {q.QuestionText}");
                }
                else
                {
                    Trace.WriteLine($"- Q {q.QuestionText}");
                }

                q.QuestionText = qm.QuestionText;
                q.ClarificationText = qm.QuestionHelpText;            // Move to SurveyQuestion?
                q.AllowMultipleAnswers = qm.AllowMultipleAnswers;      // Move to SurveyQuestion?
                q.WellKnownQuestion = qm.WellKnownQuestion;

                questionsByModelID.Add(qm.QuestionID, q);

                foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
                {
                    // See if answer choice already exists. Remember, answer choices are reusable, and can be shared across questions and surveys.

                    AnswerChoice a = _db.AnswerChoices.WhereEx(ac => ac.AnswerText == acm.AnswerChoiceText && ac.AdditionalAnswerDataFormat == acm.AdditionalAnswerDataFormat).SingleOrDefault();

                    if (a == null)
                    {
                        a = new AnswerChoice();

                        _db.AnswerChoices.Add(a);

                        Trace.WriteLine($"    + AC {a.AnswerText}");
                    }
                    else
                    {
                        Trace.WriteLine($"    - AC {a.AnswerText}");
                    }

                    a.AnswerText = acm.AnswerChoiceText;
                    a.AdditionalAnswerDataFormat = acm.AdditionalAnswerDataFormat;

                    if (!answerChoicesByModelID.ContainsKey(acm.AnswerChoiceID))
                    {
                        answerChoicesByModelID.Add(acm.AnswerChoiceID, a);
                    }
                }

            }

            // Now we can process the model into the survey-specific entities.
            // TODO: This block can now be moved into previous
            foreach (SurveyQuestionModel qm in model.Questions)
            {

                Question q = questionsByModelID[qm.QuestionID];

                SurveyQuestion sq = survey.SurveyQuestions.Where(sq2 => sq2.Question == q).SingleOrDefault();

                if (sq == null)
                {
                    sq = new SurveyQuestion
                    {
                        Question = q,
                        AnswerChoices = new List<SurveyAnswerChoice>()
                    };

                    survey.SurveyQuestions.Add(sq);

                    _db.SurveyQuestions.Add(sq);
                }

                sq.QuestionNum = qm.QuestionNum;
            }

            // Finally, with the SurveyQuestions all added and mapped, we can process AnswerChoices with forward-referenced NextQuestionID nav property

            foreach (SurveyQuestionModel qm in model.Questions)
            {

                Question q = questionsByModelID[qm.QuestionID];

                SurveyQuestion sq = survey.SurveyQuestions.Where(sq2 => sq2.Question == q).SingleOrDefault();

                foreach (SurveyQuestionAnswerChoiceModel acm in qm.AnswerChoices)
                {
                    // See if survey answer choice already exists.

                    SurveyAnswerChoice sac = sq.AnswerChoices.Where(c => c.AnswerChoice == answerChoicesByModelID[acm.AnswerChoiceID]).SingleOrDefault();

                    if (sac == null)
                    {
                        sac = new SurveyAnswerChoice
                        {
                            AnswerChoice = answerChoicesByModelID[acm.AnswerChoiceID],
                        };

                        sq.AnswerChoices.Add(sac);

                        _db.SurveyAnswerChoices.Add(sac);
                    }

                    sac.AnswerChoiceNum = acm.AnswerChoiceNum;

                    if (acm.NextQuestionID.HasValue && acm.NextQuestionID.Value != -1)
                    {
                        try
                        {
                            sac.NextSurveyQuestion = survey.SurveyQuestions.Where(ssq => ssq.Question == questionsByModelID[acm.NextQuestionID.Value]).Single();
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Couldn't find NSQ {acm.NextQuestionID} (Q='{q.QuestionText}', AC='{acm.AnswerChoiceText}')", ex);
                        }
                    }
                    else if (acm.NextQuestionID.HasValue && acm.NextQuestionID.Value == -1)
                    {
                        sac.EndSurvey = true;
                    }
                }
            }

            return survey;
        }

        public SurveyResponse ConvertToEntity(SurveyResponseModel model)
        {
            // 4326 is most common coordinate system used by GPS/Maps
            const int coordinateSystemID = 4326;

            // TODO: Validate that question and answer IDs are valid for specified survey (use ArgumentException).
            // TODO: Validate that AdditionalAnswerData matches the expected format, where applicable (use FormatException).

            var survey = _db.Surveys.Include("SurveyQuestions").Include("SurveyQuestions.AnswerChoices").Where(s => s.ID == model.SurveyID).SingleOrDefault();

            if (survey == null)
                throw new ArgumentException("Invalid SurveyID.");

            SurveyResponse response = new SurveyResponse()
            {
                Survey_ID = model.SurveyID,
                Survey_Version = model.Survey_Version,
                GPSLocation = (model.GPSLocation?.Lat != null && model.GPSLocation?.Lon != null) ? System.Data.Entity.Spatial.DbGeography.PointFromText($"Point({model.GPSLocation.Lon} {model.GPSLocation.Lat})", coordinateSystemID) : null,
                LocationNotes = model.LocationNotes,
                NearestAddress = model.NearestAddress ?? new PITCSurveyLib.Address(),
                ResponseIdentifier = model.ResponseIdentifier,
                InterviewStarted = model.StartTime,
                InterviewCompleted = model.EndTime
            };

            foreach (SurveyQuestionResponseModel qrm in model.QuestionResponses)
            {
                var question = survey.SurveyQuestions.Where(q => q.Question_ID == qrm.QuestionID).SingleOrDefault();

                if (question == null)
                    throw new ArgumentException("Invalid QuestionID for this survey.");

                foreach (SurveyQuestionAnswerChoiceResponseModel qacrm in qrm.AnswerChoiceResponses)
                {
                    var choice = question.AnswerChoices.Where(c => c.AnswerChoice_ID == qacrm.AnswerChoiceID).SingleOrDefault();

                    if (choice == null)
                        throw new ArgumentException("Invalid AnswerChoiceID for this survey question.");

                    switch (choice.AnswerChoice.AdditionalAnswerDataFormat)
                    {
                        case PITCSurveyLib.AnswerFormat.Int:
                            int intResult;
                            if (!int.TryParse(qacrm.AdditionalAnswerData, out intResult))
                                throw new FormatException("AdditionalAnswerData not parsable as 'int'.");
                            break;
                        case PITCSurveyLib.AnswerFormat.Date:
                            DateTime dateResult;
                            if (!DateTime.TryParse(qacrm.AdditionalAnswerData, out dateResult))
                                throw new FormatException("AdditionalAnswerData not parsable as 'DateTime'.");
                            break;
                    }

                    SurveyResponseAnswer answer = new SurveyResponseAnswer()
                    {
                        Question_ID = qrm.QuestionID,
                        AnswerChoice_ID = qacrm.AnswerChoiceID,
                        AdditionalAnswerData = qacrm.AdditionalAnswerData
                    };

                    response.Answers.Add(answer);
                }
            }

            return response;
        }

        #endregion

    }
}