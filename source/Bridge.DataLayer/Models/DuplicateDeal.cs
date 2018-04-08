using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class DuplicateDeal
    {
        public int Id { get; set; }
        public int DealId { get; set; }
        public int NspairId { get; set; }
        public int EwpairId { get; set; }
        public string Contract { get; set; }
        public int Declarer { get; set; }
        public int Result { get; set; }
        public int Nspercentage { get; set; }
        public int Ewpercentage { get; set; }
        public string HandViewerInput { get; set; }
        public string ContractDisplay { get; set; }
        public string Tricks { get; set; }

        public Deal Deal { get; set; }
        public SysPlayer DeclarerNavigation { get; set; }
        public Pair Ewpair { get; set; }
        public Pair Nspair { get; set; }
    }
}
