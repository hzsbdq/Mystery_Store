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
        public List<T> N_Ts { get; set; }
        public List<T> L_Ts { get; set; }
        public bool HasNextPage = false;
        protected IOrderedQueryable<T> target_ored;
        protected IQueryable<T> target_or;
        protected int amount;
        protected MySqlContext db;
        protected string rare = null;
        protected string type = null;
        protected string name = "";

        public Paging(MySqlContext db,int amount)
        {
            this.db = db;
            this.amount = amount;
            L_Ts = new List<T>();
            N_Ts = new List<T>();
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

                L_Ts = await target_ored.Skip(amount * (position)).Take(amount).ToListAsync();
                N_Ts = await target_ored.Skip(amount * (position+1)).Take(amount).ToListAsync();
               
            }
            catch(Exception e)
            {
                L_Ts = new List<T>();
                N_Ts = new List<T>();
            }

            if (N_Ts.Count > 0)
            {
                HasNextPage = true;
            }
            else
            {
                HasNextPage = false;
            }
            return L_Ts;
        }

    }
}