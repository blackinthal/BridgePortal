using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class SysPlayer
    {
        public SysPlayer()
        {
            Deal = new HashSet<Deal>();
            DuplicateDeal = new HashSet<DuplicateDeal>();
            MakeableContract = new HashSet<MakeableContract>();
        }

        public int Id { get; set; }
        public string Player { get; set; }

        public ICollection<Deal> Deal { get; set; }
        public ICollection<DuplicateDeal> DuplicateDeal { get; set; }
        public ICollection<MakeableContract> MakeableContract { get; set; }
    }
}
