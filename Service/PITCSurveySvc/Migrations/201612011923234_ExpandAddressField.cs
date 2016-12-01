namespace WeCountSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandAddressField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyResponses", "NearestAddress_Street", c => c.String());
            AddColumn("dbo.SurveyResponses", "NearestAddress_City", c => c.String());
            AddColumn("dbo.SurveyResponses", "NearestAddress_State", c => c.String());
            AddColumn("dbo.SurveyResponses", "NearestAddress_ZipCode", c => c.String());
            DropColumn("dbo.SurveyResponses", "Interviewee");
            DropColumn("dbo.SurveyResponses", "NearestAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyResponses", "NearestAddress", c => c.String());
            AddColumn("dbo.SurveyResponses", "Interviewee", c => c.String());
            DropColumn("dbo.SurveyResponses", "NearestAddress_ZipCode");
            DropColumn("dbo.SurveyResponses", "NearestAddress_State");
            DropColumn("dbo.SurveyResponses", "NearestAddress_City");
            DropColumn("dbo.SurveyResponses", "NearestAddress_Street");
        }
    }
}
