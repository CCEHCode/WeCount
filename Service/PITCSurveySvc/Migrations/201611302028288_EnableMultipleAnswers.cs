namespace WeCountSvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableMultipleAnswers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Questions", "AllowMultipleAnswers", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "AllowMultipleAnswers");
        }
    }
}
