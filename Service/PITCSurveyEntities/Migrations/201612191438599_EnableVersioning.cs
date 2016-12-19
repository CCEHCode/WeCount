namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableVersioning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Surveys", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.Surveys", "LastUpdated", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Surveys", "LastUpdated");
            DropColumn("dbo.Surveys", "Version");
        }
    }
}
