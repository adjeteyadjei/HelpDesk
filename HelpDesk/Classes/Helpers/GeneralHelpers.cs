using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelpDesk.Models;

namespace HelpDesk.Classes.Helpers
{
    public class GeneralHelpers
    {
        public string GenerateTicketCode(TicketModel tm)
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

        public int GetStatusId(string status)
        {
            switch (status)
            {
                case "Open":
                    return 1;
                case "New":
                    return 2;
                case "Solved":
                    return 3;
                default:
                    return 4;
            }
        }
    }
}