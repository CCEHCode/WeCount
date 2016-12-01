namespace WeCountSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixDataTypeTypo : DbMigration
    {
        public override void Up()
        {
			//AlterColumn("dbo.SurveyResponses", "GPSLocation", c => c.Geography());
			DropColumn("dbo.SurveyResponses", "GPSLocation");
			AddColumn("dbo.SurveyResponses", "GPSLocation", c => c.Geography());
        }
        
        public override void Down()
        {
			//AlterColumn("dbo.SurveyResponses", "GPSLocation", c => c.Geometry());
			DropColumn("dbo.SurveyResponses", "GPSLocation");
			AddColumn("dbo.SurveyResponses", "GPSLocation", c => c.Geometry());
		}
	}
}
