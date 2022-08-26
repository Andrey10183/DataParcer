using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    public class ApplicationType
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string TypeVersion { get; set; }
        public string ConfigTemplateJson { get; set; }

        public ICollection<ApplicationInstance> ApplicationInstance { get; set; }
    }
}
