namespace PITCSurveySvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tweaks : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SurveyNavigations", "NextQuestion_ID", "dbo.Questions");
            DropIndex("dbo.SurveyNavigations", new[] { "NextQuestion_ID" });
            AddColumn("dbo.SurveyNavigations", "NextSurveyQuestion_ID", c => c.Int());
            CreateIndex("dbo.SurveyNavigations", "NextSurveyQuestion_ID");
            AddForeignKey("dbo.SurveyNavigations", "NextSurveyQuestion_ID", "dbo.SurveyQuestions", "ID");
            DropColumn("dbo.Questions", "QuestionNum");
            DropColumn("dbo.SurveyNavigations", "NextQuestion_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyNavigations", "NextQuestion_ID", c => c.Int());
            AddColumn("dbo.Questions", "QuestionNum", c => c.String());
            DropForeignKey("dbo.SurveyNavigations", "NextSurveyQuestion_ID", "dbo.SurveyQuestions");
            DropIndex("dbo.SurveyNavigations", new[] { "NextSurveyQuestion_ID" });
            DropColumn("dbo.SurveyNavigations", "NextSurveyQuestion_ID");
            CreateIndex("dbo.SurveyNavigations", "NextQuestion_ID");
            AddForeignKey("dbo.SurveyNavigations", "NextQuestion_ID", "dbo.Questions", "ID");
        }
    }
}
