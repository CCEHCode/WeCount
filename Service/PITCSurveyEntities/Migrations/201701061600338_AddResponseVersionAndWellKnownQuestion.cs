namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddResponseVersionAndWellKnownQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyQuestions", "WellKnownQuestion", c => c.Int(nullable: false));
            AddColumn("dbo.SurveyResponses", "Survey_Version", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyResponses", "Survey_Version");
            DropColumn("dbo.SurveyQuestions", "WellKnownQuestion");
        }
    }
}
