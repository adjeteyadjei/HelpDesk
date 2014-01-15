using System;
using System.ComponentModel.DataAnnotations;

namespace HelpDesk.Classes.Helpers
{
    public class JsonData
    {
        [Required]
        public Object data { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public int total { get; set; }
    }
}