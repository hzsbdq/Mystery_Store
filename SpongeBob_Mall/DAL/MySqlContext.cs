using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace SpongeBob_Mall.DAL
{
    
    //[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class MySqlContext : DbContext
    {
        static MySqlContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MySqlContext>());
        }

        public MySqlContext() : base("EFContext") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<SpongeBob_Mall.Models.User> Users { get; set; }

        public System.Data.Entity.DbSet<SpongeBob_Mall.Models.Map> Maps { get; set; }

        public System.Data.Entity.DbSet<SpongeBob_Mall.Models.Goods> Goods { get; set; }

        public System.Data.Entity.DbSet<SpongeBob_Mall.Models.Admin> Admins { get; set; }

        public System.Data.Entity.DbSet<SpongeBob_Mall.Models.Order> Orders { get; set; }
    }
}