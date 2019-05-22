using SpongeBob_Mall.DAL;
using SpongeBob_Mall.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SpongeBob_Mall.Tools
{
    public class MapPaging:Paging<Map>
    {
        public MapPaging(MySqlContext db, int amount):base(db,amount)
        {
            target_or = db.Maps.Where(b=>true);
            target_ored = target_or.OrderBy(b => b.MapId);
        }

        public override async Task<List<Map>> Search(string Name)
        {
            target_or = db.Maps.Where(b => b.Name.Contains(Name));
            Ts = await Sort();
            return Ts;
        }

        public async Task<List<Map>> Choose(string rare,string type)
        {
            target_or = db.Maps.Where(b => (rare == null ? true : b.Rare == rare) && (type == null ? true : b.type == type));
            Ts = await Sort();
            return Ts;
        }

        public override async Task<List<Map>> Sort()
        {
            target_ored = target_or.OrderBy(b => true);
            Ts = await GetPageByNumberAsync(0);
            return Ts;
        }
    }
}