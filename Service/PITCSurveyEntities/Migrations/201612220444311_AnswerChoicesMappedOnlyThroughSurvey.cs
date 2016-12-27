namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerChoicesMappedOnlyThroughSurvey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QuestionAnswerChoices", "Question_ID", "dbo.Questions");
            DropForeignKey("dbo.QuestionAnswerChoices", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropIndex("dbo.QuestionAnswerChoices", new[] { "Question_ID" });
            DropIndex("dbo.QuestionAnswerChoices", new[] { "AnswerChoice_ID" });
            DropTable("dbo.QuestionAnswerChoices");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QuestionAnswerChoices",
                c => new
                    {
                        Question_ID = c.Int(nullable: false),
                        AnswerChoice_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Question_ID, t.AnswerChoice_ID });
            
            CreateIndex("dbo.QuestionAnswerChoices", "AnswerChoice_ID");
            CreateIndex("dbo.QuestionAnswerChoices", "Question_ID");
            AddForeignKey("dbo.QuestionAnswerChoices", "AnswerChoice_ID", "dbo.AnswerChoices", "ID", cascadeDelete: true);
            AddForeignKey("dbo.QuestionAnswerChoices", "Question_ID", "dbo.Questions", "ID", cascadeDelete: true);
        }
    }
}
