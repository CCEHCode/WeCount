namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SurveyAnswerChoices : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SurveyNavigations", "AnswerChoice_ID", "dbo.AnswerChoices");
            DropForeignKey("dbo.SurveyNavigations", "NextSurveyQuestion_ID", "dbo.SurveyQuestions");
            DropForeignKey("dbo.SurveyNavigations", "SurveyQuestion_ID", "dbo.SurveyQuestions");
            DropIndex("dbo.SurveyNavigations", new[] { "SurveyQuestion_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "AnswerChoice_ID" });
            DropIndex("dbo.SurveyNavigations", new[] { "NextSurveyQuestion_ID" });
            DropTable("dbo.SurveyNavigations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SurveyNavigations",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SurveyQuestion_ID = c.Int(nullable: false),
                        AnswerChoice_ID = c.Int(nullable: false),
                        NextSurveyQuestion_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.SurveyNavigations", "NextSurveyQuestion_ID");
            CreateIndex("dbo.SurveyNavigations", "AnswerChoice_ID");
            CreateIndex("dbo.SurveyNavigations", "SurveyQuestion_ID");
            AddForeignKey("dbo.SurveyNavigations", "SurveyQuestion_ID", "dbo.SurveyQuestions", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SurveyNavigations", "NextSurveyQuestion_ID", "dbo.SurveyQuestions", "ID");
            AddForeignKey("dbo.SurveyNavigations", "AnswerChoice_ID", "dbo.AnswerChoices", "ID", cascadeDelete: true);
        }
    }
}
