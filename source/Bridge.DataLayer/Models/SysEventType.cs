using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class SysEventType
    {
        public SysEventType()
        {
            Event = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string EventType { get; set; }

        public ICollection<Event> Event { get; set; }
    }
}
