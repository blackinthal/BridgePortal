using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Bridge.Domain.Models;
using Bridge.WebAPI.Models;

namespace Bridge.WebAPI.Queries
{
    public class EventQueries
    {
        private readonly BridgeContext _context;

        public EventQueries(BridgeContext context)
        {
            _context = context;
        }

        public IEnumerable<ImportedEventModel> GetEventsInMonth(int year, int month)
        {
            var importedEvents = _context.Events
                .Where(w => w.Date.Month == month && w.Date.Year == year)
                .Project().To<ImportedEventModel>().ToList();

            var noOfDaysInMonth = DateTime.DaysInMonth(year, month);
            var currentMonthEvents = new List<ImportedEventModel>();
            for (var day = 1; day <= noOfDaysInMonth; day++)
            {
                var refDate = new DateTime(year, month, day);
                if (refDate.DayOfWeek == DayOfWeek.Tuesday || refDate.DayOfWeek == DayOfWeek.Thursday ||
                    refDate.DayOfWeek == DayOfWeek.Wednesday)
                {
                    currentMonthEvents.Add(
                        importedEvents.FirstOrDefault(w => w.Day == day) ?? DefaultEvent(refDate)
                    ); 
                }
            }

            return currentMonthEvents;
        }

        private static ImportedEventModel DefaultEvent(DateTime date)
        {
            return new ImportedEventModel
            {
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                Name = string.Format("Locomotiva {0}", date.ToShortDateString()),
                IsImported = false
            };
        }

        public IEnumerable<EventModel> GetEvents()
        {
            return _context.Events.OrderBy(o => o.Date).Project().To<EventModel>();
        }
        public EventDetailModel GetEvent(int id)
        {
            return _context.Events.Where(w => w.Id == id)
                .Project().To<EventDetailModel>()
                .FirstOrDefault();
        } 
    }
}