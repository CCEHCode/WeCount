namespace PITCSurveyEntities.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeInterviewCompletedNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SurveyResponses", "InterviewCompleted", c => c.DateTimeOffset(precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SurveyResponses", "InterviewCompleted", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
