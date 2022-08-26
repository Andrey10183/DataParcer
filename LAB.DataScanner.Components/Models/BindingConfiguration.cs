namespace LAB.DataScanner.Components.Models
{
    public class BindingConfiguration
    {
        public const string Binding = "Binding";

        public string ReceiverQueue { get; set; }
        public string ReceiverExchange { get; set; }
        public string[] ReceiverRoutingKeys { get; set; }
        
        public string SenderExchange { get; set; }
        public string[] SenderRoutingKeys { get; set; }
    }
}
