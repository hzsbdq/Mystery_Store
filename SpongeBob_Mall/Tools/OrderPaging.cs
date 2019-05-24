using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SpongeBob_Mall.Tools
{
    public class OrderPaging:Paging<Order>
    {
        public OrderPaging(MySqlContext db, int amount) : base(db, amount)
        {
            target_or = db.Orders.Where(b => true);
            target_ored = target_or.OrderBy(b => b.Complete_Time);
        }

        public override async Task<List<Order>> Search(string Name)
        {
            name = Name;
            target_or = db.Orders.Where(b => b.Pay.Name.Contains(name)||b.Sell.Name.Contains(name));
            L_Ts = await Sort();
            return L_Ts;
        }

        public override async Task<List<Order>> Sort()
        {
            target_ored = target_or.OrderBy(b => true);
            L_Ts = await GetPageByNumberAsync(0);
            return L_Ts;
        }

    }
}