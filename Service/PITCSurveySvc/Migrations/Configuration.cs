namespace WeCountSvc.Migrations
{
    using System;
	using System.Collections.Generic;
	using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using PITCSurveySvc.Entities;

	internal sealed class Configuration : DbMigrationsConfiguration<PITCSurveySvc.Entities.PITCSurveyContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
		
        protected override void Seed(PITCSurveySvc.Entities.PITCSurveyContext context)
        {
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//

			var AnsYes = new AnswerChoice { Key = "Yes", AnswerText = "Yes" };
			var AnsNo = new AnswerChoice { Key = "No", AnswerText = "No" };

			var Q1 = new Question { Key = "AlreadySurveyed", QuestionText = "Have you already been surveyed about what your housing/living situation this week? This may have happened here, at a shelter, drop in center, or on the street.",
									AnswerChoices = new List<AnswerChoice>()};

			Q1.AnswerChoices.Add(AnsYes);
			Q1.AnswerChoices.Add(AnsNo);

			context.Questions.AddOrUpdate(q => q.Key,
				Q1);

			var Svy = new Survey { Name = "PIT 2017", SurveyYear = 2017, Description = "2017 PIT Youth Survey",
				SurveyQuestions = new List<SurveyQuestion>()};

			var SQ1 = new SurveyQuestion { Question = Q1, QuestionNum = "1",
											Navigation = new List<SurveyNavigation>()};

			SQ1.Navigation.Add(new SurveyNavigation { SurveyQuestion = SQ1, AnswerChoice = AnsYes, NextSurveyQuestion = null });
			SQ1.Navigation.Add(new SurveyNavigation { SurveyQuestion = SQ1, AnswerChoice = AnsNo, NextSurveyQuestion = null });

			context.SurveyQuestions.AddOrUpdate(sq => new { sq.Question_ID, sq.DependentQuestion_ID },
				SQ1);

			Svy.SurveyQuestions.AddRange(SQ1);

			Svy.SurveyQuestions.Add(SQ1);
			//Svy.SurveyQuestions.Add(SQ2)


			context.Surveys.AddOrUpdate(s => s.Name, Svy);

			context.SaveChanges();
        }

    }

	internal static class Extensions
	{
		internal static void AddRange<T>(this IList<T> Collection, IEnumerable<T> NewItems)
		{
			foreach (T Item in NewItems)
			{
				Collection.Add(Item);
			}
		}

		internal static void AddRange<T>(this IList<T> Collection, params T[] NewItems)
		{
			foreach (T Item in NewItems)
			{
				Collection.Add(Item);
			}
		}
	}
}
