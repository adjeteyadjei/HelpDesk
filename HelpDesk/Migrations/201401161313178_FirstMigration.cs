namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Priorities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        FullName = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Picture = c.Binary(),
                        DateOfBirth = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        CreatedAt = c.DateTime(),
                        UpdatedAt = c.DateTime(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: false)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProjectLeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        ProjectId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ProjectId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.ProjectTeams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: false)
                .Index(t => t.ProjectId)
                .Index(t => t.TeamId);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        CanAdd = c.Boolean(nullable: false),
                        CanEdit = c.Boolean(nullable: false),
                        CanView = c.Boolean(nullable: false),
                        CanForward = c.Boolean(nullable: false),
                        CanAssign = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CreatedById)
                .Index(t => t.TeamId)
                .Index(t => t.UpdatedById)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TeamRelations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamOneId = c.Int(nullable: false),
                        TeamTwoId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Teams", t => t.TeamTwoId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.TeamTwoId)
                .Index(t => t.UpdatedById);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Description = c.String(),
                        Code = c.String(),
                        ProjectId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        PriorityId = c.Int(nullable: false),
                        ParentTicketId = c.Int(),
                        AssignedToId = c.String(maxLength: 128),
                        AssignedById = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedById)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedToId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Priorities", t => t.PriorityId, cascadeDelete: false)
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: false)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: false)
                .ForeignKey("dbo.Types", t => t.TypeId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.AssignedById)
                .Index(t => t.AssignedToId)
                .Index(t => t.CreatedById)
                .Index(t => t.PriorityId)
                .Index(t => t.ProjectId)
                .Index(t => t.StatusId)
                .Index(t => t.TypeId)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.CreatedById)
                .Index(t => t.UpdatedById);
            
            CreateTable(
                "dbo.TicketDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        Subject = c.String(),
                        Description = c.String(),
                        Code = c.String(),
                        StatusId = c.Int(nullable: false),
                        PriorityId = c.Int(nullable: false),
                        AssignedToId = c.String(maxLength: 128),
                        AssignedById = c.String(maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        UpdatedById = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedById)
                .ForeignKey("dbo.AspNetUsers", t => t.AssignedToId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Priorities", t => t.PriorityId, cascadeDelete: false)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: false)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetUsers", t => t.UpdatedById)
                .Index(t => t.AssignedById)
                .Index(t => t.AssignedToId)
                .Index(t => t.CreatedById)
                .Index(t => t.PriorityId)
                .Index(t => t.StatusId)
                .Index(t => t.TicketId)
                .Index(t => t.UpdatedById);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TicketDetails", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketDetails", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketDetails", "StatusId", "dbo.Status");
            DropForeignKey("dbo.TicketDetails", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.TicketDetails", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketDetails", "AssignedToId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TicketDetails", "AssignedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentModels", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentModels", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Tickets", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "TypeId", "dbo.Types");
            DropForeignKey("dbo.Types", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Types", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Tickets", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Tickets", "PriorityId", "dbo.Priorities");
            DropForeignKey("dbo.Tickets", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "AssignedToId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tickets", "AssignedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommentModels", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamRelations", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamRelations", "TeamTwoId", "dbo.Teams");
            DropForeignKey("dbo.TeamRelations", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamMembers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamMembers", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamMembers", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamMembers", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Status", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Status", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.RolePermissions", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.RolePermissions", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectTeams", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.Teams", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Teams", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectTeams", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.ProjectLeaders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectLeaders", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Projects", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Priorities", "UpdatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Priorities", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TicketDetails", new[] { "UpdatedById" });
            DropIndex("dbo.TicketDetails", new[] { "TicketId" });
            DropIndex("dbo.TicketDetails", new[] { "StatusId" });
            DropIndex("dbo.TicketDetails", new[] { "PriorityId" });
            DropIndex("dbo.TicketDetails", new[] { "CreatedById" });
            DropIndex("dbo.TicketDetails", new[] { "AssignedToId" });
            DropIndex("dbo.TicketDetails", new[] { "AssignedById" });
            DropIndex("dbo.CommentModels", new[] { "UpdatedById" });
            DropIndex("dbo.CommentModels", new[] { "TicketId" });
            DropIndex("dbo.Tickets", new[] { "UpdatedById" });
            DropIndex("dbo.Tickets", new[] { "TypeId" });
            DropIndex("dbo.Types", new[] { "UpdatedById" });
            DropIndex("dbo.Types", new[] { "CreatedById" });
            DropIndex("dbo.Tickets", new[] { "StatusId" });
            DropIndex("dbo.Tickets", new[] { "ProjectId" });
            DropIndex("dbo.Tickets", new[] { "PriorityId" });
            DropIndex("dbo.Tickets", new[] { "CreatedById" });
            DropIndex("dbo.Tickets", new[] { "AssignedToId" });
            DropIndex("dbo.Tickets", new[] { "AssignedById" });
            DropIndex("dbo.CommentModels", new[] { "CreatedById" });
            DropIndex("dbo.TeamRelations", new[] { "UpdatedById" });
            DropIndex("dbo.TeamRelations", new[] { "TeamTwoId" });
            DropIndex("dbo.TeamRelations", new[] { "CreatedById" });
            DropIndex("dbo.TeamMembers", new[] { "UserId" });
            DropIndex("dbo.TeamMembers", new[] { "UpdatedById" });
            DropIndex("dbo.TeamMembers", new[] { "TeamId" });
            DropIndex("dbo.TeamMembers", new[] { "CreatedById" });
            DropIndex("dbo.Status", new[] { "UpdatedById" });
            DropIndex("dbo.Status", new[] { "CreatedById" });
            DropIndex("dbo.RolePermissions", new[] { "UpdatedById" });
            DropIndex("dbo.RolePermissions", new[] { "CreatedById" });
            DropIndex("dbo.ProjectTeams", new[] { "TeamId" });
            DropIndex("dbo.Teams", new[] { "UpdatedById" });
            DropIndex("dbo.Teams", new[] { "CreatedById" });
            DropIndex("dbo.ProjectTeams", new[] { "ProjectId" });
            DropIndex("dbo.ProjectLeaders", new[] { "UserId" });
            DropIndex("dbo.ProjectLeaders", new[] { "ProjectId" });
            DropIndex("dbo.Projects", new[] { "UpdatedById" });
            DropIndex("dbo.Projects", new[] { "CreatedById" });
            DropIndex("dbo.Priorities", new[] { "UpdatedById" });
            DropIndex("dbo.Priorities", new[] { "CreatedById" });
            DropIndex("dbo.AspNetUserClaims", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropTable("dbo.TicketDetails");
            DropTable("dbo.Types");
            DropTable("dbo.Tickets");
            DropTable("dbo.CommentModels");
            DropTable("dbo.TeamRelations");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.Status");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.Teams");
            DropTable("dbo.ProjectTeams");
            DropTable("dbo.Projects");
            DropTable("dbo.ProjectLeaders");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Priorities");
        }
    }
}
