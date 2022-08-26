using Microsoft.OData.Edm;
using Microsoft.AspNet.OData.Builder;
//using Microsoft.OData.ModelBuilder;

namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    public static class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();
            builder.EntitySet<ApplicationInstance>("ApplicationInstances").EntityType.HasKey(c => c.InstanceID);
            builder.EntitySet<ApplicationType>("ApplicationTypes").EntityType.HasKey(c => c.TypeID);
            builder.EntitySet<Binding>("Bindings").EntityType.HasKey(c => new { c.PublisherInstanceID, c.ConsumerInstanceID });
            return builder.GetEdmModel();
        }
    }
}
