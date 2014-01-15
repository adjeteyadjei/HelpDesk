using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HelpDesk.Models
{
    public class TeamRelation : AuditFileds
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TeamOneId { get; set; }
        //public virtual Team TeamOne { get; set; }
        public int TeamTwoId { get; set; }
        public virtual Team TeamTwo { get; set; }
    }
}