using EntitetsLager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLager
{
    public class ApplikationDbContext: DbContext
    {


        public DbSet<Kund> Kunder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Data Source=""sqlutb2-db.hb.se, 56077"";Initial Catalog=suht2501;Persist Security Info=True;User ID=suht2501;Password=***********;Trust Server Certificate=True");
        }
    }
}
