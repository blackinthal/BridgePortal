using System;
using System.Collections.Generic;
using System.IO;
using Bridge.Domain.EventAggregate.Commands;
using BridgeWebAPI.Providers;

namespace BridgeWebAPI.Modules
{
    public class ExtractEventMetadataModule
    {
        private readonly IEventProvider _provider;
        private readonly IUrlProvider _urlProvider;

        public List<string> Errors; 
        public ExtractEventMetadataModule(IEventProvider provider, IUrlProvider urlProvider)
        {
            _provider = provider;
            _urlProvider = urlProvider;

            Errors = new List<string>();
        }

        public ImportEvent ExtractEventMetadata(DateTime selectedDate)
        {
            var tempFilePath = _provider.ReadEventPBNData(_urlProvider.GetUrl(new DateTime(2015, 4, 28)));

            var command = ProcessPbnFile(tempFilePath);

            _provider.CleanUp(tempFilePath);

            return command;
        }

        private static ImportEvent ProcessPbnFile(string filePath)
        {
            var result = new ImportEvent();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!line.StartsWith("[Deal ")) continue;

                    var start = line.IndexOf('"');
                    var end = line.LastIndexOf('"');

                    var pbnDeal = line.Substring(start + 1, end - start - 1);
                }
            }
            return result;
        }
    }
}