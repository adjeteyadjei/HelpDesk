using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;

namespace HelpDesk.Classes.Repositories
{
    public class TeamRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();

        public JsonData Post(TeamMain newRecord, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

                        var theTeam = new Team
                        {
                            Name = newRecord.Name,
                            Description = newRecord.Description,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            CreatedById = user.Id,
                            UpdatedById = user.Id,
                            IsDeleted = false
                        };

                        db.Teams.Add(theTeam);
                        db.SaveChanges();
                        newRecord.TeamId = theTeam.Id;

                        var count = newRecord.Members.Count();
                        if (count > 0)
                        {
                            PostTeamMembers(newRecord, user, db);
                        }

                        scope.Complete();
                        return _dh.ReturnJsonData(newRecord, true, "The team has been created", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Update(TeamMain updatedRecord, User user)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

                        var oRecord = db.Teams.First(p => p.Id == updatedRecord.TeamId);

                        oRecord.Name = updatedRecord.Name;
                        oRecord.Description = updatedRecord.Description;
                        oRecord.UpdatedAt = DateTime.Now;
                        oRecord.UpdatedById = user.Id;

                        db.SaveChanges();
                        updatedRecord.TeamId = oRecord.Id;

                        var count = updatedRecord.Members.Count();
                        if (count > 0)
                        {
                            UpdateTeamMembers(updatedRecord, user, db);
                        }

                        scope.Complete();
                        return _dh.ReturnJsonData(updatedRecord, true, "Team data has been updated successfully", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public JsonData Delete(int id)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    using (var db = new DataContext())
                    {
                        var delRecord = db.Teams.First(p => p.Id == id);
                        //db.Teams.Remove(delRecord);
                        delRecord.IsDeleted = true;

                        DeleteTeamRelations(id);

                        db.SaveChanges();
                        scope.Complete();
                        return _dh.ReturnJsonData(id, true, "Team has been deleted", 1);
                    }
                }
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        /*public JsonData Get(int id)
        {
            try
            {
                var data = _db.Teams.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
                var success = false;
                var message = "";
                var total = 0;
                if (data != null)
                {
                    success = true;
                    message = "Team data loaded successfully";
                    total = 1;
                }
                return _dh.ReturnJsonData(data, success, message, total);

            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }*/

        public JsonData GetAll()
        {
            try
            {
                var data = new List<TeamMain>();
                var teams = _db.Teams.Where(p => p.IsDeleted == false).ToList();
                var total = teams.Count();
                var message = "No Team Found";
                if (total <= 0) return _dh.ReturnJsonData(data, false, message, total);

                data.AddRange(teams.Select(team => new TeamMain
                {
                    Id = team.Id,
                    TeamId = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    Members = GetAllTeamMemers(team.Id)
                }));
                //data = data.ToArray();
                message = "Teams loaded successfully";

                return _dh.ReturnJsonData(data, true, message, total);
            }
            catch (Exception e)
            {
                return _dh.ExceptionProcessor(e);
            }
        }

        public void PostTeamMembers(TeamMain newRecord, User user, DataContext db)
        {
            if (newRecord.Members == null) throw new ArgumentNullException("No members" + " selected");

            foreach (var members in newRecord.Members)
            {
                db.TeamMembers.Add(new TeamMember
                {
                    TeamId = newRecord.TeamId,
                    UserId = members.Id,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    UpdatedById = user.Id,
                    IsDeleted = false
                });
                db.SaveChanges();
            }
        }

        public void UpdateTeamMembers(TeamMain newRecord, User user, DataContext db)
        {
            if (newRecord.Members == null) throw new ArgumentNullException("No members" + " selected");

            var oldMembers = db.TeamMembers.Where(p => p.TeamId == newRecord.TeamId && !p.IsDeleted);
            foreach (var teamMember in oldMembers)
                teamMember.IsDeleted = true;
            db.SaveChanges();


            foreach (var members in newRecord.Members)
            {
                db.TeamMembers.Add(new TeamMember
                {
                    TeamId = newRecord.TeamId,
                    UserId = members.Id,
                    UpdatedAt = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    CreatedById = user.Id,
                    UpdatedById = user.Id,
                    IsDeleted = false
                });
                db.SaveChanges();
            }
        }


        /*public void UpdateTeamRelations(TeamMain updateRecord, User user)
        {
            using (var db = new DataContext())
            {
                var relations = db.TeamRelations.Where(x => x.TeamOneId == updateRecord.TeamId).ToList();
                var cnt = relations.Count();
                if (cnt != 0)
                {
                    foreach (
                        var tm in
                            relations.Select(
                                teamRelation => db.TeamRelations.FirstOrDefault(i => i.Id == teamRelation.Id))
                        )
                    {
                        if (tm != null) tm.IsDeleted = true;
                        db.SaveChanges();
                    }
                }

                var count = updateRecord.Relations.Count();
                if (count == 0)
                    return;

                foreach (var teamRel in
                    updateRecord.Relations.Select(
                        rel =>
                            db != null
                                ? new
                                {
                                    rel,
                                    r =
                                        db.TeamRelations.FirstOrDefault(
                                            x =>
                                                x.TeamOneId == updateRecord.TeamId && x.TeamTwoId == rel.TeamId &&
                                                x.IsDeleted == false)
                                }
                                : null).
                        Where(@t => @t.r == null).Select(@t => new TeamRelation
                        {
                            TeamOneId = updateRecord.TeamId,
                            TeamTwoId = @t.rel.TeamId,
                            UpdatedAt = DateTime.Now,
                            CreatedAt = DateTime.Now,
                            CreatedById = user.Id,
                            UpdatedById = user.Id,
                            IsDeleted = false
                        }))
                {
                    db.TeamRelations.Add(teamRel);
                    db.SaveChanges();
                }
            }
        }*/

        public void DeleteTeamRelations(int teamId)
        {
            using (var db = new DataContext())
            {
                var relations =
                    db.TeamRelations.Where(x => x.TeamOneId == teamId || x.TeamTwoId == teamId && x.IsDeleted == false)
                        .ToList();
                var cnt = relations.Count();
                if (cnt == 0)
                    return;

                foreach (
                    var tm in
                        relations.Select(
                            teamRelation =>
                                db != null ? db.TeamRelations.FirstOrDefault(i => i.Id == teamRelation.Id) : null)
                    )
                {
                    if (tm != null) tm.IsDeleted = true;
                    db.SaveChanges();
                }
            }
        }

        public UserModel[] GetAllTeamMemers(int teamId)
         {
             var teams = _db.TeamMembers.Where(p => p.TeamId == teamId && p.IsDeleted == false);

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
    }
}