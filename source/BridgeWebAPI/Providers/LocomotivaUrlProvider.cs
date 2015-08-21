using System;

namespace Bridge.WebAPI.Providers
{
    public class LocomotivaUrlProvider : IUrlProvider
    {
        private const string BaseUrl = "http://www.locomotiva.ro/cls";
        public string GetUrl(DateTime selectedDate)
        {
            var year = (selectedDate.Year % 2000);
            var month = selectedDate.Month.ToString("D2");
            var day = selectedDate.Day.ToString("D2");

            return string.Format("{0}/{1}/{2}/{3}-{4}-{5}.pbn",BaseUrl,year,month,day,month,year);
        }
    }
}