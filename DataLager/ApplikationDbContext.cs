using EntitetsLager;
using Microsoft.EntityFrameworkCore;

namespace DataLager
{
    public class ApplikationDbContext : DbContext
    {
        public DbSet<Kund> Kunder { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=sqlutb2-db.hb.se,56077;Initial Catalog=suht2501;User ID=suht2501;Password=RBE152;Encrypt=True;TrustServerCertificate=True");
        }
    }
}
