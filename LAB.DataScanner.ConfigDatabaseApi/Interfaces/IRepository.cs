using LAB.DataScanner.ConfigDatabaseApi.Models;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void CreateApplicationInstance(ApplicationInstance item);
        void CreateApplicationType(ApplicationType item);
        void CreateBinding(Binding item);
        void DeleteApplicationInstance(int id);
        void DeleteApplicationType(int id);
        void DeleteBinding(int keyPublisherInstanceID, int keyConsumerInstanceID);
        IQueryable<T> ReadAll();
        ApplicationInstance ReadApplicationInstancesById(int id);
        ApplicationType ReadApplicationTypeById(int id);
        Binding ReadBindingsByKeys(int keyPublisherInstanceID, int keyConsumerInstanceID);
        void UpdateApplicationInstance(int id, ApplicationInstance item);
        void UpdateApplicationType(int id, ApplicationType item);
    }
}
