using LAB.DataScanner.ConfigDatabaseApi.Interfaces;
using LAB.DataScanner.ConfigDatabaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LAB.DataScanner.ConfigDatabaseApi.DBContext
{
    public class DataScannerDbContext : DbContext, IConfigDatabaseContext
    {
        public DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<Binding> Bindings { get; set; }

        public DataScannerDbContext(DbContextOptions<DataScannerDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationType>(entity => entity.ToTable("ApplicationType","meta"));
            modelBuilder.Entity<ApplicationType>()
                .HasKey(at => at.TypeID)
                .HasName("PK_ApplicationType_TypeID");

            modelBuilder.Entity<ApplicationInstance>(entity => entity.ToTable("ApplicationInstance", "component"));
            modelBuilder.Entity<ApplicationInstance>()
                .HasKey(ai => ai.InstanceID)
                .HasName("PK_ApplicationInstance_InstanceID");
            modelBuilder.Entity<ApplicationInstance>()
                .HasOne(d => d.Type)
                .WithMany(p => p.ApplicationInstance)
                .HasForeignKey(d => d.TypeID)
                .HasConstraintName("FK_ApplicationInstance_ApplicationType")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Binding>(entity => entity.ToTable("Binding", "binding"));
            modelBuilder.Entity<Binding>()
                .HasKey(b => new { b.PublisherInstanceID, b.ConsumerInstanceID });
            modelBuilder.Entity<Binding>()
                .HasOne(d => d.ConsumerInstance)
                .WithMany(ai => ai.BindingConsumerInstances)                
                .HasForeignKey(d => d.ConsumerInstanceID)
                .HasConstraintName("FK_Binding_ApplicationInstance1_Consumer")
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Binding>()
                .HasOne(d => d.PublisherInstance)
                .WithMany(ai => ai.BindingPublisherInstances)                
                .HasForeignKey(d => d.PublisherInstanceID)
                .HasConstraintName("FK_Binding_ApplicationInstance_Publisher")
                .OnDelete(DeleteBehavior.Cascade);
        }

        public void Save() =>
            SaveChanges();

        DbSet<T> IConfigDatabaseContext.Set<T>() where T : class =>
            Set<T>();
    }
}
