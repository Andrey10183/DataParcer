using LAB.DataScanner.Components.Interfaces;
using LAB.DataScanner.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace LAB.DataScanner.Components.Services.Persisters
{
    public class PersisterDBContext : DbContext, IPersisterDBContext
    {
        public DbSet<PersisterModel> Data { get; set; }

        public PersisterDBContext(DbContextOptions<PersisterDBContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public void Save()
        {
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersisterModel>(entity => entity.ToTable("Product", "dbo"));
        }
    }
}
