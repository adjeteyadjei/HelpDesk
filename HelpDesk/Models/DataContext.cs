using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Models
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTeam> ProjectTeams { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamRelation> TeamRelations { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }
        public DbSet<ProjectLeader> ProjectLeaders { get; set; }
        public DbSet<TicketComment> TicketComments { get; set; }

    }
}