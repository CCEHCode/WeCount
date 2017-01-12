namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeviceIdToVolunteer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Volunteers", "DeviceId", c => c.Guid());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Volunteers", "DeviceId");
        }
    }
}
