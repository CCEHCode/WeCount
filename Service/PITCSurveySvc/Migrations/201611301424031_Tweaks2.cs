namespace PITCSurveySvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tweaks2 : DbMigration
    {
        public override void Up()
        {
			DropForeignKey("dbo.Questions", "Survey_ID", "dbo.Surveys");
			DropIndex("dbo.Questions", new[] { "Survey_ID" });
            DropColumn("dbo.Questions", "Survey_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "Survey_ID", c => c.Int());
            CreateIndex("dbo.Questions", "Survey_ID");
        }
    }
}
