namespace Bridge.WebAPI.Models
{
    public class EventModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public string Name { get; set; }
        public bool IsImported { get; set; }
    }
}