using System.Collections.Generic;
using Newtonsoft.Json;


namespace Bridge.WebAPI.Models
{
    public class DealDetailModel
    {
        public int Id { get; set; }
        public IEnumerable<DealResultViewModel> DealResults { get; set; }
        public IEnumerable<MakeableContractViewModel> MakeableContracts { get; set; }
        public string HandViewerInput { get; set; }
        [JsonProperty(PropertyName = "BestContract")]
        public string BestContractDisplay { get; set; }
        [JsonProperty(PropertyName = "BestContractDeclarer")]
        public string SysPlayerPlayer { get; set; }
        [JsonProperty(PropertyName = "BestContractResult")]
        public int BestContractResult { get; set; }
    }

    public class DealResultViewModel
    {
        public string Contract { get; set; }
        public string HandViewerInput { get; set; }
        public int NSPercentage { get; set; }
        public int EWPercentage { get; set; }
        public int Result { get; set; }
        [JsonProperty(PropertyName = "Declarer")]
        public string PlayerPlayer { get; set; }
        [JsonProperty(PropertyName = "NSPair")]
        public string NSPairName { get; set; }
        [JsonProperty(PropertyName = "EWPair")]
        public string EWPairName { get; set; }
    }

    public class MakeableContractViewModel
    {
        public int Level { get; set; }
        public int Denomination { get; set; }
        public string Contract { get; set; }
        [JsonProperty(PropertyName = "Declarer")]
        public string SysPlayerPlayer { get; set; }
        public string HandViewerInput { get; set; }
    }
}