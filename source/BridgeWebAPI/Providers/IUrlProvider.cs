using System;

namespace BridgeWebAPI.Providers
{
    public interface IUrlProvider
    {
        string GetUrl(DateTime selectedDate);
    }
}