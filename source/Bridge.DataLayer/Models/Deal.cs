using System;
using System.Collections.Generic;

namespace Bridge.DataLayer.Models
{
    public partial class Deal
    {
        public Deal()
        {
            DuplicateDeal = new HashSet<DuplicateDeal>();
            MakeableContract = new HashSet<MakeableContract>();
        }

        public int Id { get; set; }
        public int EventId { get; set; }
        public int Index { get; set; }
        public string Pbnrepresentation { get; set; }
        public int SysVulnerabilityId { get; set; }
        public string HandViewerInput { get; set; }
        public string BestContract { get; set; }
        public string BestContractDisplay { get; set; }
        public int BestContractDeclarer { get; set; }
        public int BestContractResult { get; set; }
        public string BestContractHandViewerInput { get; set; }

        public SysPlayer BestContractDeclarerNavigation { get; set; }
        public Event Event { get; set; }
        public SysVulnerability SysVulnerability { get; set; }
        public ICollection<DuplicateDeal> DuplicateDeal { get; set; }
        public ICollection<MakeableContract> MakeableContract { get; set; }
    }
}
