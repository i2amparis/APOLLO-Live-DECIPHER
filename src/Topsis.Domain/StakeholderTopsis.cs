using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Topsis.Domain.Common;

namespace Topsis.Domain
{
    public class StakeholderTopsis : AlternativeTopsis
    {
        public const string DefaultGroupName = "Global";

        [JsonPropertyName("uid")]
        [JsonProperty("uid")]
        public string StakeholderId { get; set; }

        [JsonPropertyName("w")]
        [JsonProperty("w")]
        public double StakeholderWeight { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public int? JobCategoryId { get; set; }

        public static StakeholderTopsis Create(string stakeholderId,
            int alternativeId,
            double topsis,
            int? jobCategoryId,
            double stakeholderWeight)
        {
            return new StakeholderTopsis
            {
                AlternativeId = alternativeId,
                StakeholderId = stakeholderId,
                StakeholderWeight = stakeholderWeight,
                Topsis = Rounder.Round(topsis),
                JobCategoryId = jobCategoryId
            };
        }
    }
}
