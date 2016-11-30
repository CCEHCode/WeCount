namespace PITCSurveySvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerChoices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        AnswerText = c.String(),
                        AdditionalAnswerData = c.String(),
                        AdditionalAnswerDataFormat = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        QuestionNum = c.String(),
                        QuestionText = c.String(),
                        ClarificationText = c.String(),
                        Survey_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Surveys", t => t.Survey_ID)
                .Index(t => t.Survey_ID);
            
            CreateTable(
                "dbo.SurveyNavigations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AnswerChoice_ID = c.Int(nullable: false),
                        NextQuestion_ID = c.Int(),
                        Question_ID = c.Int(nullable: false),
                        Survey_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AnswerChoices", t => t.AnswerChoice_ID, cascadeDelete: true)
                .ForeignKey("dbo.Questions", t => t.NextQuestion_ID)
                .ForeignKey("dbo.Questions", t => t.Question_ID, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.Survey_ID)
                .Index(t => t.AnswerChoice_ID)
                .Index(t => t.NextQuestion_ID)
                .Index(t => t.Question_ID)
                .Index(t => t.Survey_ID);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SurveyYear = c.Int(nullable: false),
                        Description = c.String(),
                        IntroText = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SurveyQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionNum = c.String(),
                        DependentQuestion_ID = c.Int(),
                        Question_ID = c.Int(),
                        Survey_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Questions", t => t.DependentQuestion_ID)
                .ForeignKey("dbo.Questions", t => t.Question_ID)
                .ForeignKey("dbo.Surveys", t => t.Survey_ID)
                .Index(t => t.DependentQuestion_ID)
                .Index(t => t.Question_ID)
                .Index(t => t.Survey_ID);
            
            CreateTable(
                "dbo.SurveyResponseAnswers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        AnswerChoice_ID = c.Int(),
                        Question_ID = c.Int(),
                        SurveyResponse_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AnswerChoices", t => t.AnswerChoice_ID)
                .ForeignKey("dbo.Questions", t => t.Question_ID)
                .ForeignKey("dbo.SurveyResponses", t => t.SurveyResponse_ID)
                .Index(t => t.AnswerChoice_ID)
                .Index(t => t.Question_ID)
                .Index(t => t.SurveyResponse_ID);
            
            CreateTable(
                "dbo.SurveyResponses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Interviewee = c.String(),
                        InterviewStarted = c.DateTimeOffset(nullable: false, precision: 7),
                        InterviewCompleted = c.DateTimeOffset(nullable: false, precision: 7),
                        GPSLocation = c.Geometry(),
                        NearestAddress = c.String(),
                        LocationNotes = c.String(),
                        Volunteer_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Volunteers", t => t.Volunteer_ID)
                .Index(t => t.Volunteer_ID);
            
            CreateTable(
                "dbo.Volunteers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        MobilePhone = c.String(),
                        HomePhone = c.String(),
                        Address_Street = c.String(),
                        Address_City = c.String(),
                        Address_State = c.String(),
                        Address_ZipCode = c.String(),
                        AuthMethod = c.Int(nullable: false),
                        AuthID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.QuestionAnswerChoices",
                c => new
                    {
                        Question_ID = c.Int(nullable: false),
                        AnswerChoice_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Question_ID, t.AnswerChoice_ID })
                .ForeignKey("dbo.Questions", t => t.Question_ID, cascadeDelete: true)
                .ForeignKey("dbo.AnswerChoices", t => t.AnswerChoice_ID, cascadeDelete: true)
                .Index(t => t.Question_ID)
                .Index(t => t.AnswerChoice_ID);
            
            CreateTable(
                "dbo.SurveyQuestionDependentAnswers",
                c => new
                    {
                        SurveyQuestion_ID = c.Int(nullable: false),
                        AnswerChoice_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SurveyQuestion_ID, t.AnswerChoice_ID })
                .ForeignKey("dbo.SurveyQuestions", t => t.SurveyQuestion_ID, cascadeDelete: true)
                .ForeignKey("dbo.AnswerChoices", t => t.AnswerChoice_ID, cascadeDelete: true)
                .Index(t => t.SurveyQuestion_ID)
                .Index(t => t.AnswerChoice_ID);
            
            CreateTable(
                "dbo.SurveyQuestionNavigation",
                c => new
                    {
                        SurveyQuestion_ID = c.Int(nullable: false),
                        SurveyNavigation_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SurveyQuestion_ID, t.SurveyNavigation_ID })
                .ForeignKey("dbo.SurveyQuestions", t => t.SurveyQuestion_ID, cascadeDelete: true)
                .ForeignKey("dbo.SurveyNavigations", t => t.SurveyNavigation_ID, cascadeDelete: true)
                .Index(t => t.SurveyQuestion_ID)
                .Index(t => t.SurveyNavigation_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyResponses", "Volunteer_ID", "dbo.Volunteers");
            DropForeignKey("dbo.SurveyResponseAnswers", "SurveyResponse_ID", "dbo.SurveyResponses");
            DropForeignKey("dbo.SurveyResponseAnswers", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyResponseAnswers", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.SurveyQuestions", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.SurveyQuestions", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyQuestionNavigation", "SurveyNavigation_ID", "dbo.SurveyNavigations");
            DropForeignKey("dbo.SurveyQuestionNavigation", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyQuestionDependentAnswers", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.SurveyQuestionDependentAnswers", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyQuestions", "DependentQuestion_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyNavigations", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.Questions", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.SurveyNavigations", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyNavigations", "NextQuestion_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyNavigations", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.QuestionAnswerChoices", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.QuestionAnswerChoices", "Question_ID", "dbo.Questions");
            DropIndex("dbo.SurveyQuestionNavigation", new[] { "SurveyNavigation_ID" });
            DropIndex("dbo.SurveyQuestionNavigation", new[] { "SurveyQuestion_ID" });
            DropIndex("dbo.SurveyQuestionDependentAnswers", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyQuestionDependentAnswers", new[] { "SurveyQuestion_ID" });
            DropIndex("dbo.QuestionAnswerChoices", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.QuestionAnswerChoices", new[] { "Question_ID" });
            DropIndex("dbo.SurveyResponses", new[] { "Volunteer_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "SurveyResponse_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "Question_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Question_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "DependentQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "Question_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "NextQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.Questions", new[] { "Survey_ID" });
            DropTable("dbo.SurveyQuestionNavigation");
            DropTable("dbo.SurveyQuestionDependentAnswers");
            DropTable("dbo.QuestionAnswerChoices");
            DropTable("dbo.Volunteers");
            DropTable("dbo.SurveyResponses");
            DropTable("dbo.SurveyResponseAnswers");
            DropTable("dbo.SurveyQuestions");
            DropTable("dbo.Surveys");
            DropTable("dbo.SurveyNavigations");
            DropTable("dbo.Questions");
            DropTable("dbo.AnswerChoices");
        }
    }
}
