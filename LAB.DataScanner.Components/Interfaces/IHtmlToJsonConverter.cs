using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Interfaces
{
    public interface IHtmlToJsonConverter
    {
        public Task<string> ConvertAsync(string htmlPage);
    }
}
