using HelpDesk.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public static DataContext MyContext = new DataContext();

        public Configuration()
            : this(new UserManager<User>(new UserStore<User>(MyContext)))
        {
            AutomaticMigrationsEnabled = true;
        }

        public Configuration(UserManager<User> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<User> UserManager { get; private set; }

        protected override void Seed(DataContext context)
        {
            var userManager = new UserManager<User>(new UserStore<User>(context))
            {
                UserValidator = new UserValidator<User>(UserManager)
                {
                    AllowOnlyAlphanumericUserNames = false
                }
            };
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            //Create Roles if they do not exist
            if (!roleManager.RoleExists("SuperAdministrator"))
            {
                roleManager.Create(new IdentityRole("SuperAdministrator"));
            }

            if (!roleManager.RoleExists("Administrator"))
            {
                roleManager.Create(new IdentityRole("Administrator"));
            }

            if (!roleManager.RoleExists("TeamLead"))
            {
                roleManager.Create(new IdentityRole("TeamLead"));
            }

            if (!roleManager.RoleExists("Member"))
            {
                roleManager.Create(new IdentityRole("Member"));
            }

            //Create admin and super admin users with password=123456
            var superAdmin = new User
            {
                Id = 1.ToString(),
                UserName = "axongeeks",
                FullName = "Fred Adu Kumi",
                Email = "oboiratti@helpdesk.com",
                PhoneNumber = "0201234567",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Picture = null,
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now

            };
            var admin = new User
            {
                Id = 2.ToString(),
                UserName = "administrator",
                FullName = "Fred Adu Kumi",
                Email = "oboiratti@helpdesk.com",
                PhoneNumber = "0201234567",
                DateOfBirth = DateTime.Now.AddYears(-20),
                Picture = null,
                IsDeleted = false,
                UpdatedAt = DateTime.Now,
                CreatedAt = DateTime.Now
            };

            if (UserManager.FindByName(admin.UserName) == null)
            {
                //Add User Admin to Role Admin
                var result = userManager.Create(admin, "Password1");
                if (result.Succeeded)
                {
                    userManager.AddToRole(admin.Id, "Administrator");
                }
            }

            if (UserManager.FindByName(superAdmin.UserName) == null)
            {
                //Add User Admin to Role Admin
                var resul = userManager.Create(superAdmin, "Password1");
                if (resul.Succeeded)
                {
                    userManager.AddToRole(superAdmin.Id, "SuperAdministrator");
                }
            }
            //,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,

            context.RolePermissions.AddOrUpdate(a=>a.RoleName,
                new RolePermission
                {
                    Id = 1,
                    RoleName = "SuperAdministrator",
                    CanAdd = true,
                    CanEdit = true,
                    CanView = true,
                    CanForward = true,
                    CanAssign = true,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new RolePermission
                {
                    Id = 2,
                    RoleName = "Administrator",
                    CanAdd = true,
                    CanEdit = true,
                    CanView = true,
                    CanForward = true,
                    CanAssign = true,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new RolePermission
                {
                    Id = 3,
                    RoleName = "TeamLead",
                    CanAdd = true,
                    CanEdit = true,
                    CanView = true,
                    CanForward = true,
                    CanAssign = true,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new RolePermission
                {
                    Id = 4,
                    RoleName = "Member",
                    CanAdd = true,
                    CanEdit = true,
                    CanView = true,
                    CanForward = false,
                    CanAssign = true,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                });

            context.Statuses.AddOrUpdate(a => a.Name,
                new Status
                {
                    Name = "New",
                    Description = "New",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Status
                {
                    Name = "Opened",
                    Description = "Opened",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                
                new Status
                {
                    Name = "Pending",
                    Description = "Pending",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Status
                {
                    Name = "Solved",
                    Description = "Solved",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Status
                {
                    Name = "Closed",
                    Description = "Closed",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                }
                );

            context.Types.AddOrUpdate(a => a.Name,
                new Models.Type
                {
                    Name = "Incident",
                    Description = "Incident",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Models.Type
                {
                    Name = "Question",
                    Description = "Question",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Models.Type
                {
                    Name = "Problem",
                    Description = "Problem",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Models.Type
                {
                    Name = "Feature Request",
                    Description = "Feature Request",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                }
                );

            context.Priorities.AddOrUpdate(a => a.Name,
                new Priority
                {
                    Name = "Urgent",
                    Description = "Urgent",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Priority
                {
                    Name = "High",
                    Description = "High",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Priority
                {
                    Name = "Medium",
                    Description = "Medium",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                },
                new Priority
                {
                    Name = "Low",
                    Description = "Low",
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = 1.ToString(),
                    UpdatedById = 1.ToString(),
                    IsDeleted = false
                }
                );

            context.SaveChanges();

            base.Seed(context);
        }
    }
}
