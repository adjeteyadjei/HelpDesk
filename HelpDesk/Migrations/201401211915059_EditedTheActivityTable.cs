namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedTheActivityTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketActivities", "CreatedById", c => c.String(maxLength: 128));
            CreateIndex("dbo.TicketActivities", "CreatedById");
            AddForeignKey("dbo.TicketActivities", "CreatedById", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketActivities", "CreatedById", "dbo.AspNetUsers");
            DropIndex("dbo.TicketActivities", new[] { "CreatedById" });
            DropColumn("dbo.TicketActivities", "CreatedById");
        }
    }
}
