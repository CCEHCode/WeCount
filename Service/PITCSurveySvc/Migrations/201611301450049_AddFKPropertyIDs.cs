namespace PITCSurveySvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKPropertyIDs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SurveyQuestionNavigation", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyQuestionNavigation", "SurveyNavigation_ID", "dbo.SurveyNavigations");
            DropForeignKey("dbo.SurveyNavigations", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyNavigations", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.SurveyQuestions", "DependentQuestion_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyResponseAnswers", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.SurveyResponseAnswers", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyResponseAnswers", "SurveyResponse_ID", "dbo.SurveyResponses");
            DropForeignKey("dbo.SurveyResponses", "Volunteer_ID", "dbo.Volunteers");
            DropIndex("dbo.SurveyNavigations", new[] { "NextSurveyQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "Question_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "DependentQuestion_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Question_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "Question_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "SurveyResponse_ID" });
            DropIndex("dbo.SurveyResponses", new[] { "Volunteer_ID" });
            DropIndex("dbo.SurveyQuestionNavigation", new[] { "SurveyQuestion_ID" });
            DropIndex("dbo.SurveyQuestionNavigation", new[] { "SurveyNavigation_ID" });
            AddColumn("dbo.SurveyNavigations", "SurveyQuestion_ID", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyResponses", "Survey_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyNavigations", "NextSurveyQuestion_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyNavigations", "Survey_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyQuestions", "DependentQuestion_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyQuestions", "Question_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyQuestions", "Survey_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyResponseAnswers", "AnswerChoice_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyResponseAnswers", "Question_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyResponseAnswers", "SurveyResponse_ID", c => c.Int(nullable: false));
            AlterColumn("dbo.SurveyResponses", "Volunteer_ID", c => c.Int(nullable: false));
            CreateIndex("dbo.SurveyNavigations", "Survey_ID");
            CreateIndex("dbo.SurveyNavigations", "SurveyQuestion_ID");
            CreateIndex("dbo.SurveyNavigations", "NextSurveyQuestion_ID");
            CreateIndex("dbo.SurveyQuestions", "Survey_ID");
            CreateIndex("dbo.SurveyQuestions", "Question_ID");
            CreateIndex("dbo.SurveyQuestions", "DependentQuestion_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "SurveyResponse_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "Question_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "AnswerChoice_ID");
            CreateIndex("dbo.SurveyResponses", "Survey_ID");
            CreateIndex("dbo.SurveyResponses", "Volunteer_ID");
            AddForeignKey("dbo.SurveyNavigations", "SurveyQuestion_ID", "dbo.SurveyQuestions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyResponses", "Survey_ID", "dbo.Surveys", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyNavigations", "Survey_ID", "dbo.Surveys", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyQuestions", "DependentQuestion_ID", "dbo.Questions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyResponseAnswers", "AnswerChoice_ID", "dbo.AnswerChoices", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyResponseAnswers", "Question_ID", "dbo.Questions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyResponseAnswers", "SurveyResponse_ID", "dbo.SurveyResponses", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyResponses", "Volunteer_ID", "dbo.Volunteers", "ID", cascadeDelete: true);
            DropColumn("dbo.SurveyNavigations", "Question_ID");
            DropTable("dbo.SurveyQuestionNavigation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SurveyQuestionNavigation",
                c => new
                    {
                        SurveyQuestion_ID = c.Int(nullable: false),
                        SurveyNavigation_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SurveyQuestion_ID, t.SurveyNavigation_ID });
            
            AddColumn("dbo.SurveyNavigations", "Question_ID", c => c.Int(nullable: false));
            DropForeignKey("dbo.SurveyResponses", "Volunteer_ID", "dbo.Volunteers");
            DropForeignKey("dbo.SurveyResponseAnswers", "SurveyResponse_ID", "dbo.SurveyResponses");
            DropForeignKey("dbo.SurveyResponseAnswers", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyResponseAnswers", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.SurveyQuestions", "DependentQuestion_ID", "dbo.Questions");
            DropForeignKey("dbo.SurveyNavigations", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.SurveyResponses", "Survey_ID", "dbo.Surveys");
            DropForeignKey("dbo.SurveyNavigations", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropIndex("dbo.SurveyResponses", new[] { "Volunteer_ID" });
            DropIndex("dbo.SurveyResponses", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "Question_ID" });
            DropIndex("dbo.SurveyResponseAnswers", new[] { "SurveyResponse_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "DependentQuestion_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Question_ID" });
            DropIndex("dbo.SurveyQuestions", new[] { "Survey_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "NextSurveyQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "SurveyQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "Survey_ID" });
            AlterColumn("dbo.SurveyResponses", "Volunteer_ID", c => c.Int());
            AlterColumn("dbo.SurveyResponseAnswers", "SurveyResponse_ID", c => c.Int());
            AlterColumn("dbo.SurveyResponseAnswers", "Question_ID", c => c.Int());
            AlterColumn("dbo.SurveyResponseAnswers", "AnswerChoice_ID", c => c.Int());
            AlterColumn("dbo.SurveyQuestions", "Survey_ID", c => c.Int());
            AlterColumn("dbo.SurveyQuestions", "Question_ID", c => c.Int());
            AlterColumn("dbo.SurveyQuestions", "DependentQuestion_ID", c => c.Int());
            AlterColumn("dbo.SurveyNavigations", "Survey_ID", c => c.Int());
            AlterColumn("dbo.SurveyNavigations", "NextSurveyQuestion_ID", c => c.Int());
            DropColumn("dbo.SurveyResponses", "Survey_ID");
            DropColumn("dbo.SurveyNavigations", "SurveyQuestion_ID");
            CreateIndex("dbo.SurveyQuestionNavigation", "SurveyNavigation_ID");
            CreateIndex("dbo.SurveyQuestionNavigation", "SurveyQuestion_ID");
            CreateIndex("dbo.SurveyResponses", "Volunteer_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "SurveyResponse_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "Question_ID");
            CreateIndex("dbo.SurveyResponseAnswers", "AnswerChoice_ID");
            CreateIndex("dbo.SurveyQuestions", "Survey_ID");
            CreateIndex("dbo.SurveyQuestions", "Question_ID");
            CreateIndex("dbo.SurveyQuestions", "DependentQuestion_ID");
            CreateIndex("dbo.SurveyNavigations", "Survey_ID");
            CreateIndex("dbo.SurveyNavigations", "Question_ID");
            CreateIndex("dbo.SurveyNavigations", "NextSurveyQuestion_ID");
            AddForeignKey("dbo.SurveyResponses", "Volunteer_ID", "dbo.Volunteers", "ID");
            AddForeignKey("dbo.SurveyResponseAnswers", "SurveyResponse_ID", "dbo.SurveyResponses", "ID");
            AddForeignKey("dbo.SurveyResponseAnswers", "Question_ID", "dbo.Questions", "ID");
            AddForeignKey("dbo.SurveyResponseAnswers", "AnswerChoice_ID", "dbo.AnswerChoices", "ID");
            AddForeignKey("dbo.SurveyQuestions", "DependentQuestion_ID", "dbo.Questions", "ID");
            AddForeignKey("dbo.SurveyNavigations", "Survey_ID", "dbo.Surveys", "ID");
            AddForeignKey("dbo.SurveyNavigations", "Question_ID", "dbo.Questions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyQuestionNavigation", "SurveyNavigation_ID", "dbo.SurveyNavigations", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyQuestionNavigation", "SurveyQuestion_ID", "dbo.SurveyQuestions", "ID", cascadeDelete: true);
        }
    }
}
