using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitetsLager.Entiteter;
using System.Reflection.Emit;
namespace DataLager.DataLager


{

    public class ApplikationDbContext : DbContext
    {

        // Skapa en konstruktor för att skicka in options (som Johannes gör i föreläsningen)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            // Använd sedan den tilldelade Connectionstringen vi får ifrån lärarlaget. 
            optionsBuilder.UseSqlServer(@"Data Source=sqlutb2-db.hb.se,56077;Initial Catalog=oosu2516;Persist Security Info=True;User ID=oosu2516;Password=CRP175;Encrypt=True;Trust Server Certificate=True"); ;
            base.OnConfiguring(optionsBuilder);
        }


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
        //Skapar en DbSet för varje entitet i databasen när de callas. 

        public DbSet<Medlem> Medlem { get; set; }
        public DbSet<MedlemTräningspass> MedlemTräningspass { get; set; }
        public DbSet<Träningspass> Träningspass { get; set; }
        public object Medlemmar { get; set; }

    }
}
