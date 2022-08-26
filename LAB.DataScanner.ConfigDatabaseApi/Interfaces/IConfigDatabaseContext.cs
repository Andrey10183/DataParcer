using LAB.DataScanner.ConfigDatabaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LAB.DataScanner.ConfigDatabaseApi.Interfaces
{
    public interface IConfigDatabaseContext
    {
        public DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<Binding> Bindings { get; set; }

        DbSet<T> Set<T>() where T : class;

        public void Save();
    }
}
