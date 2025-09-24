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
                .WithMany(r => r.Bokningar)
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Kund)
                .WithMany(k => k.Bokningar)
                .HasForeignKey(b => b.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Bord)
                .WithMany(bord => bord.Bokningar)
                .HasForeignKey(b => b.BordID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bokning>()
                .HasOne(b => b.Anvandare)
                .WithMany(a => a.Bokningar)
                .HasForeignKey(b => b.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            // Bestallning relationships
            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Restaurang)
                .WithMany(r => r.Bestallningar)
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Kund)
                .WithMany(k => k.Bestallningar)
                .HasForeignKey(b => b.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.AnvandareBeh)
                .WithMany(a => a.Bestallningar)
                .HasForeignKey(b => b.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Bestallning>()
                .HasOne(b => b.Bokning)
                .WithMany(bok => bok.Bestallningar)
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
                .WithMany(b => b.Transaktioner)
                .HasForeignKey(t => t.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            // Other relationships
            modelBuilder.Entity<Bord>()
                .HasOne(b => b.Restaurang)
                .WithMany(r => r.Bord)
                .HasForeignKey(b => b.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RestaurangMeny>()
                .HasOne(rm => rm.Restaurang)
                .WithMany(r => r.RestaurangMenyer)
                .HasForeignKey(rm => rm.RestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<RestaurangMeny>()
                .HasOne(rm => rm.Meny)
                .WithMany(m => m.RestaurangMenyer)
                .HasForeignKey(rm => rm.MenyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BestallningsRad>()
                .HasOne(br => br.Bestallning)
                .WithMany(b => b.BestallningsRader)
                .HasForeignKey(br => br.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BestallningsRad>()
                .HasOne(br => br.Meny)
                .WithMany(m => m.BestallningsRader)
                .HasForeignKey(br => br.MenyID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LojalitetsTransaktion>()
                .HasOne(lt => lt.Kund)
                .WithMany(k => k.LojalitetsTransaktioner)
                .HasForeignKey(lt => lt.KundID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LojalitetsTransaktion>()
                .HasOne(lt => lt.Bestallning)
                .WithMany()
                .HasForeignKey(lt => lt.BestallningsID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Systemlogg>()
                .HasOne(s => s.Anvandare)
                .WithMany(a => a.Systemloggar)
                .HasForeignKey(s => s.AnvandarID)
                .OnDelete(DeleteBehavior.NoAction);

            // Lägg till Region-relationer
            modelBuilder.Entity<Restaurang>()
                .HasOne(r => r.Region)
                .WithMany(reg => reg.Restauranger)
                .HasForeignKey(r => r.RegionID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Kund>()
                .HasOne(k => k.Region)
                .WithMany(reg => reg.Kunder)
                .HasForeignKey(k => k.RegionID)
                .OnDelete(DeleteBehavior.NoAction);

            // Lägg till Anvandare-relationer
            modelBuilder.Entity<Anvandare>()
                .HasOne(a => a.Hemmarestaurang)
                .WithMany(r => r.Anvandare)
                .HasForeignKey(a => a.HemmarestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Kund>()
                .HasOne(k => k.Hemmarestaurang)
                .WithMany()
                .HasForeignKey(k => k.HemmarestaurangID)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
