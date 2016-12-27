namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SurveyAnswerChoices2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SurveyAnswerChoices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SurveyQuestion_ID = c.Int(nullable: false),
                        AnswerChoice_ID = c.Int(nullable: false),
                        AnswerChoiceNum = c.String(),
                        NextSurveyQuestion_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AnswerChoices", t => t.AnswerChoice_ID, cascadeDelete: true)
                .ForeignKey("dbo.SurveyQuestions", t => t.SurveyQuestion_ID, cascadeDelete: true)
                .ForeignKey("dbo.SurveyQuestions", t => t.NextSurveyQuestion_ID)
                .Index(t => t.SurveyQuestion_ID)
                .Index(t => t.AnswerChoice_ID)
                .Index(t => t.NextSurveyQuestion_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SurveyAnswerChoices", "NextSurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyAnswerChoices", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyAnswerChoices", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropIndex("dbo.SurveyAnswerChoices", new[] { "NextSurveyQuestion_ID" });
            DropIndex("dbo.SurveyAnswerChoices", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyAnswerChoices", new[] { "SurveyQuestion_ID" });
            DropTable("dbo.SurveyAnswerChoices");
        }
    }
}
