namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthProvider : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Volunteers", "AuthProvider", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Volunteers", "AuthProvider");
        }
    }
}
