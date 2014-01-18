namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTableName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CommentModels", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentModels", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.CommentModels", "UpdatedById", "dbo.AspNetUsers");
            DropIndex("dbo.CommentModels", new[] { "CreatedById" });
            DropIndex("dbo.CommentModels", new[] { "TicketId" });
            DropIndex("dbo.CommentModels", new[] { "UpdatedById" });
            CreateTable(
                "dbo.TicketComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        TicketId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedById);
            
            DropTable("dbo.CommentModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CommentModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Comment = c.String(),
                        TicketId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.TicketComments", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketComments", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketComments", "CreatedById", "dbo.AspNetUsers");
            DropIndex("dbo.TicketComments", new[] { "UpdatedById" });
            DropIndex("dbo.TicketComments", new[] { "TicketId" });
            DropIndex("dbo.TicketComments", new[] { "CreatedById" });
            DropTable("dbo.TicketComments");
            CreateIndex("dbo.CommentModels", "UpdatedById");
            CreateIndex("dbo.CommentModels", "TicketId");
            CreateIndex("dbo.CommentModels", "CreatedById");
            AddForeignKey("dbo.CommentModels", "UpdatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.CommentModels", "TicketId", "dbo.Tickets", "Id", cascadeDelete: false);
            AddForeignKey("dbo.CommentModels", "CreatedById", "dbo.AspNetUsers", "Id");
        }
    }
}
