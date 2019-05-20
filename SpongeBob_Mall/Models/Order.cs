using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int Price { get; set; }
        public DateTime Complete_Time { get; set; }

        [Column("Sell_UserId")]
        [Required]
        public virtual User Sell { set; get; }
        [Column("Pay_UserId")]
        [Required]
        public virtual User Pay { set; get; }
    }
}