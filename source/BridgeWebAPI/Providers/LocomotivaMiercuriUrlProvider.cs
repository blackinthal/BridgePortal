using System;
using System.Globalization;

namespace Bridge.WebAPI.Providers
{
    public class LocomotivaMiercuriUrlProvider : IUrlProvider
    {
        private const string BaseUrl = "http://www.locomotiva.ro/cls";
        public string GetUrl(DateTime selectedDate)
        {
            var year = (selectedDate.Year % 2000);
            var month = selectedDate.Month.ToString("D2");
            var day = selectedDate.Day.ToString("D2");

            var dayOfWeek = GetDayName(selectedDate.DayOfWeek);

            return string.Format("{0}/{1}/{2}/{3}-{4}-{5}.pbn", BaseUrl, year, dayOfWeek, day, month, year);
        }

        private static string GetDayName(DayOfWeek dayOfWeek)
        {
            var info = new CultureInfo("RO-ro").DateTimeFormat;
            return info.GetDayName(dayOfWeek).ToLower();
        }
    }
}