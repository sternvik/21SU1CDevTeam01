using EntitetsLager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataLager
{
    /// <summary>
    /// Databasens huvudkontextklass som hanterar entiteter och databasanslutning.
    /// Ärver från DbContext och används av Entity Framework Core.
    /// </summary>
    public class ApplikationDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /// <summary>
            /// Konfigurerar databsen genom att ange connectionstring.
            /// </summary>

            optionsBuilder.UseSqlServer(@"Data Source=sqlutb2-db.hb.se,56077;Initial Catalog=oosu2513;Persist Security Info=True;User ID=oosu2513;Password=OUL850;Encrypt=True;Trust Server Certificate=True");
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Konfigurerar entitetsrelationer vid skapande av databasen.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfigurera många-till-många-relationen mellan Medlem och Träningspass.
            modelBuilder.Entity<MedlemTräningspass>()
                .HasKey(t => new { t.TräningspassID, t.MedlemID });

            modelBuilder.Entity<MedlemTräningspass>()
                .HasOne(me => me.Medlem).WithMany(m => m.MedlemTräningspass).HasForeignKey(me => me.MedlemID);

            modelBuilder.Entity<MedlemTräningspass>()
                .HasOne(tr => tr.Träningspass).WithMany(t => t.MedlemTräningspass).HasForeignKey(tr => tr.TräningspassID);


            base.OnModelCreating(modelBuilder);
        }

        // DbSet för att hantera respektive entitetstabell i databasen
        public DbSet<Medlem> Medlemar { get; set; }
        public DbSet<Tränare> Tränare { get; set; }
        public DbSet<Träningspass> Träningspass { get; set; }
        public DbSet<Utlåning> Utlåningar { get; set; }
        public DbSet<Utrustning> Utrustning { get; set; }
        public DbSet<MedlemTräningspass> MedlemTräningspass { get; set; }
    }
}
