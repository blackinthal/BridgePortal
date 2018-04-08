using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class Pair
    {
        public Pair()
        {
            DuplicateDealEwpair = new HashSet<DuplicateDeal>();
            DuplicateDealNspair = new HashSet<DuplicateDeal>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int EventId { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public Event Event { get; set; }
        public ICollection<DuplicateDeal> DuplicateDealEwpair { get; set; }
        public ICollection<DuplicateDeal> DuplicateDealNspair { get; set; }
    }
}
