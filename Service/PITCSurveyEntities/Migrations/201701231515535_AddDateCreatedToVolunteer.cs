namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateCreatedToVolunteer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Volunteers", "DateCreated", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Volunteers", "DateCreated");
        }
    }
}
