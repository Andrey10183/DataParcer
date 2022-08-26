using LAB.DataScanner.Components.Models;
using Microsoft.EntityFrameworkCore;

namespace LAB.DataScanner.Components.Interfaces
{
    public interface IPersisterDBContext
    {
        DbSet<PersisterModel> Data { get; set; }

        void Save();
    }
}
