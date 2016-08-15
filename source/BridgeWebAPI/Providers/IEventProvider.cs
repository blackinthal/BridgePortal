using System.Threading.Tasks;

namespace Bridge.WebAPI.Providers
{
    public interface IEventProvider
    {
        /// <summary>
        /// Returns a path to a temporary file that contains metatadata about the event
        /// </summary>
        /// <param name="url">Url from where to read data</param>
        /// <returns>Returns a path to a temporary file that contains metatadata about the event</returns>
        Task<string> ReadEventPBNData(string url);

        void CleanUp(string temporaryFilePath);
    }
}