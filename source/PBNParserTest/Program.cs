using System;
using System.IO;

namespace PBNParserTest
{
    class Program
    {
        static void Main()
        {
            const string pbnFile = @"C:\Users\mpopescu\Desktop\sample.txt";
            var boards = 0;

            using (var reader = new StreamReader(pbnFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("[Deal ")) continue;

                    var start = line.IndexOf('"');
                    var end = line.LastIndexOf('"');

                    var pbnDeal = line.Substring(start + 1, end - start - 1);

                    if (pbnDeal.Length <= 0) continue;

                    Console.WriteLine(pbnDeal);
                    boards++;
                }
            }

            Console.WriteLine("{0} Boards", boards);
            Console.ReadLine();
        }
    }
}
