namespace LittleLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "DocumentName", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Documents", "FileName", c => c.String());
            DropColumn("dbo.Documents", "FeedbackFromTeacherToStudent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "FeedbackFromTeacherToStudent", c => c.String());
            AlterColumn("dbo.Documents", "FileName", c => c.String(maxLength: 255));
            DropColumn("dbo.Documents", "DocumentName");
        }
    }
}
