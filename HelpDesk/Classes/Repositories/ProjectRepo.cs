using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using Microsoft.Ajax.Utilities;

namespace HelpDesk.Classes.Repositories
{
    public class ProjectRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();

        public JsonData Post(ProjectModel newRecord, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                        var theProject = new Project
                        {
                            Name = newRecord.Name,
                            Description = newRecord.Description,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            CreatedById = user.Id,
                            UpdatedById = user.Id,
                            IsDeleted = false,
                            IsActive = newRecord.IsActive
                        };

                        db.Projects.Add(theProject);
                        db.SaveChanges();

                        var count = newRecord.Teams.Count();
                        if (count <= 0)
                            throw new Exception("The project should belong to a team. Please select a team(s)");

                        CreateProjectTeams(newRecord.Teams, theProject.Id, db);

                        count = newRecord.Leaders.Count();
                        if (count <= 0)
                            throw new Exception("The project should have at least a leader. Please select a leader(s)");

                        CreateProjectLeaders(newRecord.Leaders, theProject.Id, db);

                        scope.Complete();
                        return _dh.ReturnJsonData(theProject, true, "The project has been created", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public void CreateProjectTeams(TeamMain[] teams, int projectId, DataContext db)
        {
            foreach (var team in teams)
                {
                    db.ProjectTeams.Add(new ProjectTeam
                    {
                        TeamId = team.TeamId,
                        ProjectId = projectId,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false
                    });
                    db.SaveChanges();
                }
        }

        public void CreateProjectLeaders(UserModel[] leaders, int projectId, DataContext db)
        {
            foreach (var leader in leaders)
            {
                db.ProjectLeaders.Add(new ProjectLeader
                {
                    UserId = leader.Id,
                    ProjectId = projectId,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    IsDeleted = false
                });
                db.SaveChanges();
            }
        }

        public JsonData Update(ProjectModel updatedRecord, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                        var oRecord =
                            db.Projects.FirstOrDefault(p => p.Id == updatedRecord.Id && !p.IsDeleted && p.IsActive);

                        if (oRecord == null) throw new Exception("This project is no longer in the system");

                        oRecord.Name = updatedRecord.Name;
                        oRecord.Description = updatedRecord.Description;
                        oRecord.UpdatedAt = DateTime.Now;
                        oRecord.UpdatedById = user.Id;
                        oRecord.IsActive = updatedRecord.IsActive;
                        db.SaveChanges();

                        var count = updatedRecord.Teams.Count();
                        if (count <= 0)
                            throw new Exception("The project should belong to a team. Please select a team(s)");

                        UpdateProjectTeams(updatedRecord.Teams, oRecord.Id, db);

                        count = updatedRecord.Leaders.Count();
                        if (count <= 0)
                            throw new Exception("The project should have at least a leader. Please select a leader(s)");

                        UpdateProjectLeaders(updatedRecord.Leaders, oRecord.Id, db);

                        scope.Complete();
                        return _dh.ReturnJsonData(oRecord, true, "Project has been updated successfully", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public void UpdateProjectTeams(TeamMain[] teams, int projectId, DataContext db)
        {
            var pTeams = db.ProjectTeams.Where(p => p.ProjectId == projectId && p.IsDeleted == false);
            foreach (var pTeam in pTeams)
            {
                pTeam.IsDeleted = true;
                db.SaveChanges();
            }
            CreateProjectTeams(teams, projectId, db);
        }

        public void UpdateProjectLeaders(UserModel[] leaders, int projectId, DataContext db)
        {
            var pLeaders = db.ProjectLeaders.Where(p => p.ProjectId == projectId && p.IsDeleted == false);
            foreach (var pLeader in pLeaders)
            {
                pLeader.IsDeleted = true;
                db.SaveChanges();
            }
            CreateProjectLeaders(leaders, projectId, db);
        }

        public JsonData Delete(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        var delRecord = db.Projects.First(p => p.Id == id);
                        
                        delRecord.IsDeleted = true;
                        
                        DeleteProjectTeams(id, db);
                        DeleteProjectLeaders(id, db);

                        db.SaveChanges();
                        scope.Complete();
                        return _dh.ReturnJsonData(id, true, "Project has been deleted", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData GetAll(Filter filters, User user)
        {
            try
            {
                using (var db = new DataContext())
                {
                    var projects = db.Projects.Where(p => p.IsDeleted == false);
                    var message = "No Project was Found";
                    if (!projects.Any()) return _dh.ReturnJsonData(null, false, message, 0);
                    
                    var data = new List<ProjectModel>();
                    data.Clear();
                    var ps = FilterProjects(projects.ToList(), filters, user, db);
                    data.AddRange(ps.Select(project => new ProjectModel
                    {
                        ProjectId = project.Id,
                        Id = project.Id,
                        Name = project.Name,
                        Description = project.Description,
                        IsActive = project.IsActive,
                        Teams = GetProjectTeams(project.Id,db),
                        Leaders = GetProjectLeaders(project.Id,db),
                    }));
                    message = "Projects loaded successfully";
                    return _dh.ReturnJsonData(data, true, message, data.Count());
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public TeamMain[] GetProjectTeams(int projectId, DataContext db)
        {
            var teams = db.ProjectTeams.Where(p => p.ProjectId == projectId && p.IsDeleted == false).ToList();
            return teams.Select(team => new TeamMain
            {
                Id = team.Team.Id, 
                Name = team.Team.Name, 
                Description = team.Team.Description, 
                Members = GetProjectTeamMemers(team.Team.Id, db)
            }).ToArray();
        }

        public UserModel[] GetProjectTeamMemers(int teamId, DataContext db)
        {
            var teams = db.TeamMembers.Where(p => p.TeamId == teamId && p.IsDeleted == false);

            return
                teams.Select(
                    teamMember =>
                        new UserModel
                        {
                            Id = teamMember.User.Id,
                            UserName = teamMember.User.UserName,
                            FullName = teamMember.User.FullName,
                            Email = teamMember.User.Email,
                            PhoneNumber = teamMember.User.PhoneNumber,
                            Picture = teamMember.User.Picture,
                            DateOfBirth = teamMember.User.DateOfBirth,
                            IsDeleted = false,
                            CreatedAt = teamMember.CreatedAt,
                            UpdatedAt = teamMember.CreatedAt
                        }).ToArray();
        }

        public UserModel[] GetProjectLeaders(int projectId, DataContext db)
        {
            var leaders = db.ProjectLeaders.Where(p => p.ProjectId == projectId && p.IsDeleted == false).ToList();

            return leaders.Select(leader => new UserModel
            {
                Id = leader.User.Id,
                UserName = leader.User.UserName,
                FullName = leader.User.FullName,
                Email = leader.User.Email,
                PhoneNumber = leader.User.PhoneNumber,
                Picture = leader.User.Picture,
                DateOfBirth = leader.User.DateOfBirth,
                IsDeleted = false,
                CreatedAt = leader.CreatedAt,
                UpdatedAt = leader.CreatedAt
            }).ToArray();
        }

        public void DeleteProjectTeams(int projectId, DataContext db)
        {
            var projectTeams = db.ProjectTeams.Where(x => x.ProjectId == projectId).ToList();
            foreach (
                var pt in
                    projectTeams.Select(projectTeam => db.ProjectTeams.First(x => x.ProjectId == projectTeam.ProjectId))
                )
            {
                pt.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public void DeleteProjectLeaders(int projectId, DataContext db)
        {
            var projectLeaders = db.ProjectLeaders.Where(x => x.ProjectId == projectId).ToList();
            foreach (
                var pt in
                    projectLeaders.Select(projectLeader => db.ProjectLeaders.First(x => x.ProjectId == projectLeader.ProjectId))
                )
            {
                pt.IsDeleted = true;
                db.SaveChanges();
            }
        }

        public List<Project> FilterProjects(List<Project> projects, Filter filters, User user, DataContext db)
        {
            /*var firstOrDefault = db.TeamMembers.FirstOrDefault(p => p.UserId == user.Id && p.IsDeleted == false);
            if (firstOrDefault != null)
            {
                var team = firstOrDefault.Team;
                var data = new List<Project>();
                    data.Clear();
                var teamProjects = db.ProjectTeams.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList();
                /*foreach (var teamProject in teamProjects)
                {
                    data.AddRange(projects.Where(p=>p.Id == teamProject.ProjectId).ToList());
                }#1#
                projects = teamProjects.Aggregate(projects, (current, teamProject) => current.Where(p =>
                    p.Id == teamProject.ProjectId).ToList());
            }*/
            var tms = db.TeamMembers.Where(p => p.UserId == user.Id && p.IsDeleted == false);
            if (tms.Any())
            {
                foreach (var teamProjects in tms.Select(teamMember => teamMember.Team).Select(team => db.ProjectTeams.Where(p => p.TeamId == team.Id && p.IsDeleted == false).ToList()))
                {
                    projects.AddRange(teamProjects.Aggregate(projects, (current, teamProject) => current.Where(p =>
                        p.Id == teamProject.ProjectId).ToList()));
                }
            }
            projects = projects.DistinctBy(p => p.Name).ToList();

            if (filters != null && filters.ProjectId != 0)
            {
                projects = projects.Where(p => p.Id == filters.ProjectId).ToList();
            }

            return projects;
        }

        
    }
}