namespace LAB.DataScanner.Components.Models
{
    public class RmqConfiguration
    {
        public const string RmqConfig = "RmqConfig";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string HostName { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
    }
}
