using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class Event
    {
        public Event()
        {
            Deal = new HashSet<Deal>();
            Pair = new HashSet<Pair>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int SysEventTypeId { get; set; }
        public bool IsImported { get; set; }

        public SysEventType SysEventType { get; set; }
        public ICollection<Deal> Deal { get; set; }
        public ICollection<Pair> Pair { get; set; }
    }
}
