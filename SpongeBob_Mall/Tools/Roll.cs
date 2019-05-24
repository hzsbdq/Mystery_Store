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
    public class Roll
    {
        private MySqlContext db;
        int num;
        int num2;
        string type;
        List<Map> maps;
        public Roll(MySqlContext db)
        {
            this.db = db;
        }

        public async Task<Map> StartRoll()
        {
            num = ToRoll(1, 100);
            if (num == 16)
            {
                type = "至宝";
            }else if (num >= 10 && num <= 15)
            {
                type = "神话";
            }
            else if (num >= 1 && num <= 9)
            {
                type = "不朽";
            }
            else if (num >= 17 && num <= 35)
            {
                type = "稀有";
            }
            else
            {
                type = "普通";
            }
            maps = await db.Maps.Where(b => b.Rare == type).ToListAsync();
            num2 = ToRoll(0, maps.Count);
            return maps[num2];
        }

        public int ToRoll(int a,int b)
        {
            Random ran = new Random(System.DateTime.Now.Millisecond+num+num2);
            return ran.Next(a, b);
        }
    }
}