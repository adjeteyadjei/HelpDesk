using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Models;

namespace HelpDesk.Classes.Helpers
{
    public static class GeneralHelpers
    {
        public static string GenerateTicketCode(TicketModel tm)
        {
            string code;
            switch (tm.Type.Name)
            {
                case "Incident":
                    code = "INC";
                    break;
                case "Question":
                    code = "QUE";
                    break;
                case "Problem":
                    code = "PRO";
                    break;
                default:
                    code = "FRE";
                    break;
            }
            return code + DateTime.Now.ToFileTime();
        }

        public static int GetStatusId(string status)
        {
            switch (status)
            {
                case "Opened":
                    return 2;
                case "New":
                    return 1;
                case "Solved":
                    return 4;
                case "Pending":
                    return 3;
                default:
                    return 5;
            }
        }

        public static string GetStatus(int i)
        {
            switch (i)
            {
                case 2:
                    return "Opened";
                case 1:
                    return "New";
                case 4:
                    return "Solved";
                case 3:
                    return "Pending";
                default:
                    return "Closed";
            }
        }
    }
}