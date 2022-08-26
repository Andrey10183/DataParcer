using System;

namespace LAB.DataScanner.ConfigDatabaseApi.Infrastructure
{
    public class DbEntityBoundedException : Exception
    {
        public DbEntityBoundedException(string message) : base(message) { }
    }
}
