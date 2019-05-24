using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SpongeBob_Mall.Tools
{
    public class UserPaging:Paging<User>
    {
        public UserPaging(MySqlContext db, int amount) : base(db, amount)
        {
            target_or = db.Users.Where(b => b.Sex!=3);
            target_ored = target_or.OrderBy(b => b.UserId);
        }

        public override async Task<List<User>> Search(string Name)
        {
            name = Name;
            target_or = db.Users.Where(b => b.Name.Contains(name)&&b.Sex!=3);
            L_Ts = await Sort();
            return L_Ts;
        }

        public override async Task<List<User>> Sort()
        {
            target_ored = target_or.OrderBy(b => true);
            L_Ts = await GetPageByNumberAsync(0);
            return L_Ts;
        }
    }
}