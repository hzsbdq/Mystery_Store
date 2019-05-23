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
            name = Name;
            target_or = db.Maps.Where(b => (this.rare == null ? true : b.Rare == this.rare) && (this.type == null ? true : b.type == this.type) && b.Name.Contains(name));
            L_Ts = await Sort();
            return L_Ts;
        }

        public async Task<List<Map>> Choose(string rare,string type)
        {
            this.rare = rare ?? this.rare;
            this.type = type ?? this.type;
            this.rare = rare == "不限" ? null : this.rare;
            this.type = type == "不限" ? null : this.type;
            target_or = db.Maps.Where(b => (this.rare == null ? true : b.Rare == this.rare) && (this.type == null ? true : b.type == this.type)&& b.Name.Contains(name));
            L_Ts = await Sort();
            return L_Ts;
        }

        public override async Task<List<Map>> Sort()
        {
            target_ored = target_or.OrderBy(b => true);
            L_Ts = await GetPageByNumberAsync(0);
            return L_Ts;
        }
    }
}