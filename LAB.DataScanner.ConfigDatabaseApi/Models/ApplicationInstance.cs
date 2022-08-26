using System.Collections.Generic;

namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    public class ApplicationInstance
    {
        public int InstanceID { get; set; }
        public int TypeID { get; set; }        
        public string InstanceName { get; set; }
        public string ConfigJson { get; set; }

        public ApplicationType Type { get; set; }
        public ICollection<Binding> BindingPublisherInstances { get; set; }
        public ICollection<Binding> BindingConsumerInstances { get; set; }
    }
}
