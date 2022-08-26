using LAB.DataScanner.ConfigDatabaseApi.Infrastructure;
using LAB.DataScanner.ConfigDatabaseApi.Interfaces;
using LAB.DataScanner.ConfigDatabaseApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace LAB.DataScanner.ConfigDatabaseApi.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private IConfigDatabaseContext _context;

        public Repository(IConfigDatabaseContext context) =>
            _context = context;

        public IQueryable<T> ReadAll() =>
            _context.Set<T>();

        public ApplicationInstance ReadApplicationInstancesById(int id) =>
            _context.ApplicationInstances.Where(x => x.InstanceID == id)
            .Include(x => x.BindingPublisherInstances)
            .Include(x => x.BindingConsumerInstances)
            .FirstOrDefault();

        public ApplicationType ReadApplicationTypeById(int id) =>
            _context.ApplicationTypes.Where(x => x.TypeID == id)
            .Include(x => x.ApplicationInstance)
            .FirstOrDefault();

        public Binding ReadBindingsByKeys(int keyPublisherInstanceID, int keyConsumerInstanceID) =>
            _context.Bindings.Where(
                x => x.PublisherInstanceID == keyPublisherInstanceID &&
                x.ConsumerInstanceID == keyConsumerInstanceID).FirstOrDefault();

        public void CreateApplicationInstance(ApplicationInstance item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (ReadApplicationTypeById(item.TypeID) == null)
                throw new DbItemNotExistException($"TypeID = {item.TypeID} not found!");

            _context.ApplicationInstances.Add(item);
            _context.Save();
        }

        public void CreateApplicationType(ApplicationType item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            _context.ApplicationTypes.Add(item);
            _context.Save();
        }

        public void CreateBinding(Binding item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.PublisherInstanceID == item.ConsumerInstanceID)
                throw new ArgumentException($"Publisher and consumer instances can't be the same!");

            _ = _context.ApplicationInstances.Find(item.PublisherInstanceID) ??
                throw new DbItemNotExistException($"Not found application instance with id = {item.PublisherInstanceID}");

            _ = _context.ApplicationInstances.Find(item.ConsumerInstanceID) ??
                throw new DbItemNotExistException($"Not found application instance with id = {item.ConsumerInstanceID}");

            if (ReadBindingsByKeys(item.PublisherInstanceID, item.ConsumerInstanceID) != null)
                throw new DbItemAlreadyExist($"Binding with publisher Id = {item.PublisherInstanceID} and consumer id = {item.ConsumerInstanceID} already exist!");

            _context.Bindings.Add(item);
            _context.Save();
        }

        public void UpdateApplicationInstance(int id, ApplicationInstance item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var current = ReadApplicationInstancesById(id) ??
                throw new DbItemNotExistException($"There is no application instance with id = {id}");

            if (ReadApplicationTypeById(item.TypeID) == null)
                throw new DbItemNotExistException($"There is no application type with id = {item.TypeID}");

            current.InstanceName = item.InstanceName;
            current.TypeID = item.TypeID;
            current.ConfigJson = item.ConfigJson;

            _context.ApplicationInstances.Update(current);
            _context.Save();
        }

        public void UpdateApplicationType(int id, ApplicationType item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            var current = ReadApplicationTypeById(id) ??
                throw new DbItemNotExistException($"There is no application type with id = {id}");

            current.TypeName = item.TypeName;
            current.TypeVersion = item.TypeVersion;
            current.ConfigTemplateJson = item.ConfigTemplateJson;

            _context.ApplicationTypes.Update(current);
            _context.Save();
        }

        public void DeleteApplicationInstance(int id)
        {
            var current = ReadApplicationInstancesById(id) ??
                throw new DbItemNotExistException($"There is no application instance with id = {id}");

            if (current.BindingPublisherInstances.Count > 0 ||
                current.BindingConsumerInstances.Count > 0)
                throw new DbEntityBoundedException($"This InstanceID = {id} binded to antother application(s)");

            _context.ApplicationInstances.Remove(current);
            _context.Save();
        }

        public void DeleteApplicationType(int id)
        {
            var current = ReadApplicationTypeById(id) ??
                throw new DbItemNotExistException($"There is no application type with id = {id}");

            if (current.ApplicationInstance.Count > 0)
                throw new DbEntityBoundedException($"This TypeID = {id} binded to antother application(s) instances");

            _context.ApplicationTypes.Remove(current);
            _context.Save();

        }

        public void DeleteBinding(int keyPublisherInstanceID, int keyConsumerInstanceID)
        {
            var current = ReadBindingsByKeys(keyPublisherInstanceID, keyConsumerInstanceID) ??
                throw new DbItemNotExistException($"Binding with publisher Id = {keyPublisherInstanceID} and consumer id = {keyConsumerInstanceID} not exist!");

            _context.Bindings.Remove(current);
            _context.Save();
        }
    }
}
