namespace WeCountSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tweaks3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surveys", "Active", c => c.Boolean(nullable: false));
            AddColumn("dbo.SurveyResponseAnswers", "AdditionalAnswerData", c => c.String());
            AddColumn("dbo.SurveyResponses", "ResponseIdentifier", c => c.Guid(nullable: false));
            DropColumn("dbo.AnswerChoices", "AdditionalAnswerData");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnswerChoices", "AdditionalAnswerData", c => c.String());
            DropColumn("dbo.SurveyResponses", "ResponseIdentifier");
            DropColumn("dbo.SurveyResponseAnswers", "AdditionalAnswerData");
            DropColumn("dbo.Surveys", "Active");
        }
    }
}
