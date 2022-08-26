using System.Collections.Generic;

namespace LAB.DataScanner.Components.Models
{
    public class ConverterConfiguration
    {
        public const string ConverterConfig = "Application";

        public string HtmlFragmentStrategy { get; set; }

        public Dictionary<string, string> HtmlFragmentExpressions { get; set; }
    }
}
