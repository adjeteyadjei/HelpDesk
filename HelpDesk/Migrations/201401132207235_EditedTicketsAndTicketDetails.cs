namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EditedTicketsAndTicketDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketDetails", "Subject", c => c.String());
            AddColumn("dbo.TicketDetails", "Code", c => c.String());
            AddColumn("dbo.TicketDetails", "PriorityId", c => c.Int(nullable: false));
            AddColumn("dbo.TicketDetails", "AssignedToId", c => c.String(maxLength: 128));
            AddColumn("dbo.TicketDetails", "AssignedById", c => c.String(maxLength: 128));
            AddColumn("dbo.Tickets", "Subject", c => c.String());
            AddColumn("dbo.Tickets", "Description", c => c.String());
            AddColumn("dbo.Tickets", "ProjectId", c => c.Int(nullable: false));
            CreateIndex("dbo.TicketDetails", "AssignedById");
            CreateIndex("dbo.TicketDetails", "AssignedToId");
            CreateIndex("dbo.TicketDetails", "PriorityId");
            CreateIndex("dbo.Tickets", "ProjectId");
            AddForeignKey("dbo.TicketDetails", "AssignedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TicketDetails", "AssignedToId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TicketDetails", "PriorityId", "dbo.Priorities", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects", "Id", cascadeDelete: true);
            DropColumn("dbo.Tickets", "Title");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "Title", c => c.String());
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.TicketDetails", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.TicketDetails", "AssignedToId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketDetails", "AssignedById", "dbo.AspNetUsers");
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            DropIndex("dbo.TicketDetails", new[] { "PriorityId" });
            DropIndex("dbo.TicketDetails", new[] { "AssignedToId" });
            DropIndex("dbo.TicketDetails", new[] { "AssignedById" });
            DropColumn("dbo.Tickets", "ProjectId");
            DropColumn("dbo.Tickets", "Description");
            DropColumn("dbo.Tickets", "Subject");
            DropColumn("dbo.TicketDetails", "AssignedById");
            DropColumn("dbo.TicketDetails", "AssignedToId");
            DropColumn("dbo.TicketDetails", "PriorityId");
            DropColumn("dbo.TicketDetails", "Code");
            DropColumn("dbo.TicketDetails", "Subject");
        }
    }
}
