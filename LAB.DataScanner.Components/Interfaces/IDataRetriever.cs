using System.Threading.Tasks;

namespace LAB.DataScanner.Components.Interfaces
{
    public interface IDataRetriever
    {
        Task<byte[]> RetrieveBytesAsync(string uri);

        Task<string> RetrieveStringAsync(string uri);
    }
}
