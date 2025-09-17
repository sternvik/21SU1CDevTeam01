using EntitetsLager;
using Microsoft.EntityFrameworkCore;

namespace DataLager
{
    public class ApplikationDbContext : DbContext
    {
        public DbSet<Kund> Kunder { get; set; }
        public DbSet<Region> Regioner { get; set; }
        public DbSet<Restaurang> Restauranger { get; set; }
        public DbSet<Anvandare> Anvandare { get; set; }
        public DbSet<Bord> Bord { get; set; }
        public DbSet<Bokning> Bokningar { get; set; }
        public DbSet<Meny> Menyer { get; set; }
        public DbSet<RestaurangMeny> RestaurangMenyer { get; set; }
        public DbSet<Bestallning> Bestallningar { get; set; }
        public DbSet<BestallningsRad> BestallningsRader { get; set; }
        public DbSet<Transaktion> Transaktioner { get; set; }
        public DbSet<LojalitetsTransaktion> LojalitetsTransaktioner { get; set; }
        public DbSet<Systemlogg> Systemloggar { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=sqlutb2-db.hb.se,56077;Initial Catalog=suht2501;User ID=suht2501;Password=RBE152;Encrypt=True;TrustServerCertificate=True");
        }
    }
}
