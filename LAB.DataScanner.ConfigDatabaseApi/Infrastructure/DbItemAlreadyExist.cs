using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LAB.DataScanner.ConfigDatabaseApi.Infrastructure
{
    public class DbItemAlreadyExist : Exception
    {
        public DbItemAlreadyExist(string message) : base(message) { }
    }
}
