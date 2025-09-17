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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurera foreign key relationships för att undvika cykliska cascade paths
            // Sätt alla foreign keys till NO ACTION för att undvika problem

            // Bokning relationships
            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Restaurang)
                .WithMany()
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Kund)
                .WithMany()
                .HasForeignKey(b => b.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Bord)
                .WithMany()
                .HasForeignKey(b => b.BordID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Anvandare)
                .WithMany()
                .HasForeignKey(b => b.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            // Bestallning relationships
            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Restaurang)
                .WithMany()
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Kund)
                .WithMany()
                .HasForeignKey(b => b.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.AnvandareBeh)
                .WithMany()
                .HasForeignKey(b => b.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Bokning)
                .WithMany()
                .HasForeignKey(b => b.BokningsID)
                .OnDelete(DeleteBehavior.NoAction);

            // Transaktion relationships
            modelBuilder.Entity<Transaktion>()
                .HasOne(t => t.Restaurang)
                .WithMany()
                .HasForeignKey(t => t.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaktion>()
                .HasOne(t => t.Anvandare)
                .WithMany()
                .HasForeignKey(t => t.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaktion>()
                .HasOne(t => t.Bestallning)
                .WithMany()
                .HasForeignKey(t => t.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            // Other relationships
            modelBuilder.Entity<Bord>()
                .HasOne(b => b.Restaurang)
                .WithMany()
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RestaurangMeny>()
                .HasOne(rm => rm.Restaurang)
                .WithMany()
                .HasForeignKey(rm => rm.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RestaurangMeny>()
                .HasOne(rm => rm.Meny)
                .WithMany()
                .HasForeignKey(rm => rm.MenyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BestallningsRad>()
                .HasOne(br => br.Bestallning)
                .WithMany()
                .HasForeignKey(br => br.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BestallningsRad>()
                .HasOne(br => br.Meny)
                .WithMany()
                .HasForeignKey(br => br.MenyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LojalitetsTransaktion>()
                .HasOne(lt => lt.Kund)
                .WithMany()
                .HasForeignKey(lt => lt.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LojalitetsTransaktion>()
                .HasOne(lt => lt.Bestallning)
                .WithMany()
                .HasForeignKey(lt => lt.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Systemlogg>()
                .HasOne(s => s.Anvandare)
                .WithMany()
                .HasForeignKey(s => s.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
