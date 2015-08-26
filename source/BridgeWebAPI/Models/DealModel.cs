using System.Collections.Generic;


namespace Bridge.WebAPI.Models
{
    public class DealDetailModel
    {
        public int Id { get; set; }
        public IEnumerable<DealResultViewModel> DealResults { get; set; }
        public IEnumerable<MakeableContractViewModel> MakeableContracts { get; set; }
    }

    public class DealResultViewModel
    {
        
    }

    public class MakeableContractViewModel
    {
        
    }
}