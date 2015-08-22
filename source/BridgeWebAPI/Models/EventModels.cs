using System.Collections.Generic;

namespace Bridge.WebAPI.Models
{
    public class ImportedEventModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public bool IsImported { get; set; }
    }

    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysEventTypeEventType { get; set; }
    }

    public class EventDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SysEventTypeEventType { get; set; }
        public IEnumerable<PairModel> Pairs { get; set; }
        public IEnumerable<DealModel> Deals { get; set; } 
    }

    public class PairModel
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public decimal Score { get; set; }
    }

    public class DealModel
    {
        public int Index { get; set; }
        public string PBNRepresentation { get; set; }
    }
}