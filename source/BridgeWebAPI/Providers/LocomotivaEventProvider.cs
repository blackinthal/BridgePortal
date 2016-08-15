using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Bridge.WebAPI.Providers
{
    public class LocomotivaEventProvider : IEventProvider
    {
        public async Task<string> ReadEventPBNData(string url)
        {
            var tempPath = Path.GetTempFileName();

            var request = WebRequest.Create(url);

            var response = await request.GetResponseAsync() as HttpWebResponse;

            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return tempPath;

            var stream = response.GetResponseStream();

            if (stream == null)
                return tempPath;

            var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync();

            using (var writer = new StreamWriter(tempPath))
            {
                await writer.WriteAsync(contents);
            }

            return tempPath;
        }

        public void CleanUp(string temporaryFilePath)
        {
            if(File.Exists(temporaryFilePath))
                File.Delete(temporaryFilePath);
        }
    }
}