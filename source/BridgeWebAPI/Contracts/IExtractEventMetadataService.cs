using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Bridge.Domain.EventAggregate.Commands;

namespace Bridge.WebAPI.Contracts
{
    public interface IExtractEventMetadataService
    {
        Task<ImportEvent> ExtractEventMetadata(DateTime selectedDate);
    }
}