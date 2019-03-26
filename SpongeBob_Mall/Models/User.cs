using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.Models
{
    public class User
    {
        public int UserId { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string Name { set; get; }
        public int Sex { set; get; }
        public int Property { set; get; }

    }
}