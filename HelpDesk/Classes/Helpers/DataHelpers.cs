using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using HelpDesk.Models;
using Newtonsoft.Json;
using WebGrease;
using System.Net;
using System.Net.Mail;
using SendGridMail;
using SendGridMail.Transport;


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

        public Settings ReadSettngs()
        {
            //const string path = @"~\Content\config\settings.json";
            var filePath = HttpContext.Current.Server.MapPath("~/Content/config/settings.json");
            using (var reader = new StreamReader(filePath))
            {
                var settings = reader.ReadToEnd();
                var values = JsonConvert.DeserializeObject<Settings>(settings);

                return values;
            }
        }

        public void SendMail(TicketModel ticket, User user)
        {
            var dh = new DataHelpers();
            string error;

            var settings = dh.ReadSettngs();
            var message = String.Format(settings.message, ticket.AssignedTo.FullName, user.FullName, ticket.Code, ticket.Subject, ticket.Description);

            if (settings.sendmail)
            {
                SendMail(settings.smtp, settings.port, settings.username, settings.password, settings.from,
                    ticket.AssignedTo.Email, settings.subject, message, out error);
            }/*
            if (settings.sendmail)
            {
                SendMailWithSendGrid(settings.from,ticket.AssignedTo.Email, settings.subject, message, out error);
            }*/

        }

        public bool SendMail(string smtp, int port, string username, string password, string from, string to,
            string subject, string message)
        {
            string error;
            return SendMail(smtp, port, username, password, from, to, subject, message, out error);
        }

        public bool SendMail(string smtp, int port, string username, string password,
            string from, string to, string subject, string message, out string error)
        {
            try
            {
                var cred = new NetworkCredential("biggash730@gmail.com", "kp0l3miah");
                var mailClient = new SmtpClient(smtp, port) { EnableSsl = true, Credentials = cred };
                var m = new MailMessage(from, to, subject, message)
                {
                    IsBodyHtml = true,
                    DeliveryNotificationOptions =
                        DeliveryNotificationOptions.OnFailure | DeliveryNotificationOptions.OnSuccess |
                        DeliveryNotificationOptions.Delay
                };
                mailClient.Send(m);
                error = "";
                return true;
            }
            catch (Exception ex)
            {
                error = ExceptionProcessor(ex).message;
                return false;
            }
        }

        public bool SendMailWithSendGrid(string from, string to, string subject, string message, out string error)
        {
            try
            {
                // Create the email object first, then add the properties.
                var myMessage = SendGrid.GetInstance();

                // Add the message properties.
                myMessage.From = new MailAddress(from);

                // Add multiple addresses to the To field.
                var recipients = new List<String> {to};
                myMessage.AddTo(recipients);

                myMessage.Subject = subject;

                //Add the HTML and Text bodies
                myMessage.Text = message;

                // Create credentials, specifying your user name and password.
                var credentials = new NetworkCredential("biggash730", "kp0l3m1ah");

                // Create an SMTP transport for sending email.
                var transportSmtp = SMTP.GetInstance(credentials);

                // Send the email.
                transportSmtp.Deliver(myMessage);

                error = "";
                return true;
            }
            catch (Exception ex)
            {
                error = ExceptionProcessor(ex).message;
                return false;
            }
        }
    }
}