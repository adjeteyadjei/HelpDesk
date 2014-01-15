using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class LoginDetails
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}