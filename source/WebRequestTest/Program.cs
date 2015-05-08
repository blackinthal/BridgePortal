
using System;
using System.IO;
using System.Net;

namespace WebRequestTest
{
    class Program
    {
        static void Main()
        {
            const string url = "http://www.locomotiva.ro/cls/15/04/28-04-15.pbn";

            var request = WebRequest.Create(url);

            var response = request.GetResponse() as HttpWebResponse;

            if (response == null || response.StatusCode != HttpStatusCode.OK)
                return;

            var stream = response.GetResponseStream();

            if (stream == null)
                return;

            var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();

            using (var writer = new StreamWriter(@"C:\Users\mpopescu\Desktop\sample.txt"))
            {
                writer.Write(contents);
            }
            Console.WriteLine("Success");
            Console.ReadKey();
        }
    }
}
