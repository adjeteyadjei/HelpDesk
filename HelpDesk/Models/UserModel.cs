using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HelpDesk.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OtherNames { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] Picture { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        //public int CreatedById { get; set; }
        //public int UpdatedById { get; set; }
        //public bool IsLeader { get; set; }
        public bool IsAdmin { get; set; }
        public int TeamId { get; set; }
        public virtual TeamMain TeamMain { get; set; }
        public virtual Team Team { get; set; }
        public List<string> Roles { get; set; }
    }
}