using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public int UserId { get; set; }
        public int Power { get; set; }

        public virtual User User { set; get; }
    }
}