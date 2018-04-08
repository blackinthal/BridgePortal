using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class MakeableContract
    {
        public int MakeableContractId { get; set; }
        public int DealId { get; set; }
        public int Level { get; set; }
        public int Denomination { get; set; }
        public string Contract { get; set; }
        public int Declarer { get; set; }
        public string HandViewerInput { get; set; }

        public Deal Deal { get; set; }
        public SysPlayer DeclarerNavigation { get; set; }
    }
}
