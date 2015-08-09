using System;

namespace Bridge.WebAPI.Models
{
    public class EventModel
    {
        public DateTime Date;
        public string Name { get; set; }
        public bool IsImported { get; set; }
    }
}