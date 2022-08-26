namespace LAB.DataScanner.Components.Models
{
    public class UrlConfiguration
    {
        public const string Application = "Application";

        public string UrlTemplate { get; set; }
        public string[] Sequences { get; set; }
    }
}
