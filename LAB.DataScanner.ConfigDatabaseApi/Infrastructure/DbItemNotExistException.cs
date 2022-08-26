using System;

namespace LAB.DataScanner.ConfigDatabaseApi.Infrastructure
{
    public class DbItemNotExistException : Exception
    {
        public DbItemNotExistException(string message) : base(message) { }
    }
}
