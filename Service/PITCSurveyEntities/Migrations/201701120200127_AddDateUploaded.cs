namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateUploaded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyResponses", "DateUploaded", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyResponses", "DateUploaded");
        }
    }
}
