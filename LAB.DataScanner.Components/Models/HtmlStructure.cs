using System.Collections.Generic;
using System.Linq;

namespace LAB.DataScanner.Components.Models
{
    public class HtmlStructure
    {
        public string NodeType { get; set; }

        public string Tag { get; set; }

        public string Text { get; set; }

        public IDictionary<string, object> Attr { get; set; }

        public IEnumerable<HtmlStructure> Children { get; set; } = Enumerable.Empty<HtmlStructure>();
    }
}
