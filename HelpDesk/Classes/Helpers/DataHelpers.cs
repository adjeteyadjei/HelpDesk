using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Helpers;
using HelpDesk.Models;
using WebGrease;

namespace HelpDesk.Classes.Helpers
{
    public class DataHelpers
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public JsonData ReturnJsonData(object data = null, bool success = false, string message = "", int total = 0)
        {
            return new JsonData
            {
                data = data,
                success = success,
                message = message,
                total = total,
            };
        }

        public JsonData ExceptionProcessor(Exception exception)
        {
            var text = "";
            var validationException = exception as DbEntityValidationException;
            if (validationException != null)
            {
                var lines = validationException.EntityValidationErrors.Select(
                    x => new
                    {
                        name = x.Entry.Entity.GetType().Name.Split('_')[0],
                        errors = x.ValidationErrors.Select(y => y.PropertyName + ":" + y.ErrorMessage)
                    })
                                               .Select(x => string.Format("{0} => {1}", x.name, string.Join(",", x.errors)));
                text = string.Join("\r\n", lines);

                //Logger.Error(text);
                return ReturnJsonData(null, false, text);
            }

            //for any other exception, just get the full message
            text = GetErrorMessages(exception).Aggregate((a, b) => a + "\r\n" + b);
            //Logger.Error(text);
            return ReturnJsonData(null, false, text);
        }

        private static IEnumerable<string> GetErrorMessages(Exception exception)
        {
            if (exception.InnerException != null)
                foreach (var msg in GetErrorMessages(exception.InnerException))
                    yield return msg;
            yield return exception.Message;
        }

        /*public Json FilterData(Json data, Filter filters, string model)
        {
            if (data == null) return null;
            // filter the data here

            return data;
        }*/

        public List<string> GetRoles()
        {
            var roles = new List<string>
            {
               "SuperAdministrator","Administrator","TeamLead","Member" 
            };
            return roles;
        }

        public List<string> GetTypes()
        {
            var roles = new List<string>
            {
               "Incident","Question","Problem","Feature Request" 
            };
            return roles;
        }

        public List<string> GetStatuses()
        {
            var roles = new List<string>
            {
               "New","Opened","Pending","Solved","Closed"
            };
            return roles;
        }

        public List<string> GetPriorities()
        {
            var roles = new List<string>
            {
               "Urgent","High","Medium","Low" 
            };
            return roles;
        }
    }
}