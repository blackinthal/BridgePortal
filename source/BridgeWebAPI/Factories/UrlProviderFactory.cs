using System;
using Bridge.WebAPI.Providers;

namespace Bridge.WebAPI.Factories
{
    public class UrlProviderFactory
    {
        public IUrlProvider GetUrlProvider(DateTime selectedDate)
        {
            if (selectedDate.DayOfWeek == DayOfWeek.Wednesday)
                return new LocomotivaMiercuriUrlProvider();

            return new LocomotivaUrlProvider();
        }
    }
}