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
    public class Paging<T>
    {
        public List<T> Ts { get; set; }
        protected IOrderedQueryable<T> target_ored;
        protected IQueryable<T> target_or;
        protected int amount;
        protected MySqlContext db;

        public Paging(MySqlContext db,int amount)
        {
            this.db = db;
            this.amount = amount;
        }

        //搜索
        public virtual async Task<List<T>> Search(string Name)
        {

            return await Sort();
        }

        //排序
        public virtual async Task<List<T>> Sort()
        {
            return await GetPageByNumberAsync(0);
        }
        //分页
        public async Task<List<T>> GetPageByNumberAsync(int position)
        {
            try
            {
                Ts = await target_ored.Skip(amount * (position)).Take(amount).ToListAsync();
            }
            catch(Exception e)
            {
                Ts = new List<T>();
            }
            
            return Ts;
        }

    }
}