namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyResponses", "DeviceId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SurveyResponses", "DeviceId");
        }
    }
}
