namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTicketActivityModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AgentId = c.String(maxLength: 128),
                        Action = c.String(),
                        TicketId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AgentId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: false)
                .Index(t => t.AgentId)
                .Index(t => t.TicketId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketActivities", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketActivities", "AgentId", "dbo.AspNetUsers");
            DropIndex("dbo.TicketActivities", new[] { "TicketId" });
            DropIndex("dbo.TicketActivities", new[] { "AgentId" });
            DropTable("dbo.TicketActivities");
        }
    }
}
