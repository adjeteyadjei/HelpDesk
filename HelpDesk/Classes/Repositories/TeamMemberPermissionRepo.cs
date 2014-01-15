using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Classes.Helpers;
using HelpDesk.Models;
using NLog;

namespace HelpDesk.Classes.Repositories
{
    public class TeamMemberPermissionRepo
    {
        private readonly DataHelpers _dh = new DataHelpers();
        private readonly DataContext _db = new DataContext();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        //public JsonData Post(TeamMemberPermission newRecord)
        //{
        //    try
        //    {
        //        if (newRecord == null) throw new ArgumentNullException("The new" + " record is null");

        //        newRecord.UpdatedAt = DateTime.Now;
        //        newRecord.CreatedAt = DateTime.Now;
        //        newRecord.IsDeleted = false;
        //        _db.TeamMemberPermissions.Add(newRecord);
        //        _db.SaveChanges();

        //        return _dh.ReturnJsonData(newRecord, true, "Team Member Permissions have been successfully set", 1);
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(LogLevel.Error, e.ToString);
        //        return _dh.ReturnJsonData(null, false, e.Message);
        //    }
        //}

        //public JsonData Update(TeamMemberPermission updatedRecord)
        //{
        //    try
        //    {
        //        if (updatedRecord == null) throw new ArgumentNullException("The update" + " record is null");

        //        var oRecord = _db.TeamMemberPermissions.First(p => p.Id == updatedRecord.Id);
        //        oRecord.TeamMemberId = updatedRecord.TeamMemberId;
        //        oRecord.CanAdd = updatedRecord.CanAdd;
        //        oRecord.CanAssign = updatedRecord.CanAssign;
        //        oRecord.CanView = updatedRecord.CanView;
        //        oRecord.CanEdit = updatedRecord.CanEdit;
        //        oRecord.CanForward = updatedRecord.CanForward;
        //        oRecord.UpdatedAt = DateTime.Now;
        //        _db.SaveChanges();

        //        return _dh.ReturnJsonData(oRecord, true, "Team Member Permissions have been updated successfully", 1);

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(LogLevel.Error, e.ToString);
        //        return _dh.ReturnJsonData(null, false, e.ToString());
        //    }
        //}

        //public JsonData Delete(int id)
        //{
        //    try
        //    {
        //        var delRecord = _db.TeamMemberPermissions.First(p => p.Id == id);
        //        _db.TeamMemberPermissions.Remove(delRecord);
        //        //delRecord.IsDeleted = true;
        //        _db.SaveChanges();

        //        return _dh.ReturnJsonData(delRecord, true, "Team Member Permissions have been cancelled", 1);

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(LogLevel.Error, e.ToString);
        //        var message = e.Message;
        //        return _dh.ReturnJsonData(null, false, message);
        //    }
        //}

        //public JsonData Get(int id)
        //{
        //    try
        //    {
        //        var data = _db.TeamMemberPermissions.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        //        var success = false;
        //        var message = "Permissions have not been set";
        //        var total = 0;
        //        if (data != null)
        //        {
        //            success = true;
        //            message = "Team Member Permissions loaded successfully";
        //            total = 1;
        //        }
        //        return _dh.ReturnJsonData(data, success, message, total);

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(LogLevel.Error, e.ToString);
        //        return _dh.ReturnJsonData(null, false, e.ToString());
        //    }
        //}

        //public JsonData GetAll()
        //{
        //    try
        //    {
        //        var data = _db.TeamMemberPermissions.Where(p => p.IsDeleted == false);
        //        var total = data.Count();
        //        var success = false;
        //        var message = "Permissions can not be found";
        //        if (total > 0)
        //        {
        //            success = true;
        //            message = "Team Member Permissions loaded successfully";
        //        }
        //        return _dh.ReturnJsonData(data, success, message, total);

        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Log(LogLevel.Error, e.ToString);
        //        return _dh.ReturnJsonData(null, false, e.ToString());
        //    }
        //}
    }
}