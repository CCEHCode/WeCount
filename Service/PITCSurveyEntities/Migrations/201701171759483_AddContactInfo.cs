namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddContactInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContactInfos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        ContactInfo_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ContactInfos", t => t.ContactInfo_ID)
                .Index(t => t.ContactInfo_ID);
            
            AddColumn("dbo.Surveys", "ContactInfo_ID", c => c.Int());
            CreateIndex("dbo.Surveys", "ContactInfo_ID");
            AddForeignKey("dbo.Surveys", "ContactInfo_ID", "dbo.ContactInfos", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Surveys", "ContactInfo_ID", "dbo.ContactInfos");
            DropForeignKey("dbo.Contacts", "ContactInfo_ID", "dbo.ContactInfos");
            DropIndex("dbo.Surveys", new[] { "ContactInfo_ID" });
            DropIndex("dbo.Contacts", new[] { "ContactInfo_ID" });
            DropColumn("dbo.Surveys", "ContactInfo_ID");
            DropTable("dbo.Contacts");
            DropTable("dbo.ContactInfos");
        }
    }
}
