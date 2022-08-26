namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    public class Binding
    {
        public int PublisherInstanceID { get; set; }
        public int ConsumerInstanceID { get; set; }

        public ApplicationInstance PublisherInstance { get; set; }
        public ApplicationInstance ConsumerInstance { get; set; }
    }
}
