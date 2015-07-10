using System;
using System.IO;
using System.Net;

namespace BridgeWebAPI.Modules
{
    public class ExtractEventMetadataModule
    {
        private readonly IEventProvider _provider;
        private readonly IUrlProvider _urlProvider;

        public ExtractEventMetadataModule(IEventProvider provider, IUrlProvider urlProvider)
        {
            _provider = provider;
            _urlProvider = urlProvider;
        }
    }

    public interface IUrlProvider
    {
        string GetUrl(DateTime selectedDate);
    }
    public interface IEventProvider
    {
        /// <summary>
        /// Returns a path to a temporary file that contains metatadata about the event
        /// </summary>
        /// <param name="url">Url from where to read data</param>
        /// <returns>Returns a path to a temporary file that contains metatadata about the event</returns>
        string ReadEventPBNData(string url);

        void CleanUp(string temporaryFilePath);
    }

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

    public class LocomotivaUrlProvider : IUrlProvider
    {
        private const string BaseUrl = "http://www.locomotiva.ro/cls";
        public string GetUrl(DateTime selectedDate)
        {
            var year = (selectedDate.Year % 2000);
            var month = selectedDate.Month.ToString("D2");
            var day = selectedDate.Day;

            return string.Format("{0}/{1}/{2}/{3}-{4}-{5}.pbn",BaseUrl,year,month,day,month,year);
        }
    }
}