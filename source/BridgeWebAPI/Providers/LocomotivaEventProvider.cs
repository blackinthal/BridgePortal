using System.IO;
using System.Net;

namespace BridgeWebAPI.Providers
{
    public class LocomotivaEventProvider : IEventProvider
    {
        public string ReadEventPBNData(string url)
        {
            var tempPath = Path.GetTempFileName();

            var request = WebRequest.Create(url);

            var response = request.GetResponse() as HttpWebResponse;

            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return tempPath;

            var stream = response.GetResponseStream();

            if (stream == null)
                return tempPath;

            var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();

            using (var writer = new StreamWriter(tempPath))
            {
                writer.Write(contents);
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