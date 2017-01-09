namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveWellKnownQuestionToQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "WellKnownQuestion", c => c.Int(nullable: false));
            DropColumn("dbo.SurveyQuestions", "WellKnownQuestion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyQuestions", "WellKnownQuestion", c => c.Int(nullable: false));
            DropColumn("dbo.Questions", "WellKnownQuestion");
        }
    }
}
