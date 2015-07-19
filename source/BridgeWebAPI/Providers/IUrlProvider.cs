using System;

namespace Bridge.WebAPI.Providers
{
    public interface IUrlProvider
    {
        string GetUrl(DateTime selectedDate);
    }
}