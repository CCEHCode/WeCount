namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndSurveyProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyAnswerChoices", "EndSurvey", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyAnswerChoices", "EndSurvey");
        }
    }
}
