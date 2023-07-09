using System.Text.Json.Serialization;

namespace UsersAPI.Models.Incoming.Divisions
{
    public class DivisionUpdateDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        [JsonPropertyName("company_id")]
        public int CompanyId { get; set; }

        [JsonPropertyName("prefix_id")]
        public int? DivisionPrefixId { get; set; }

        [JsonPropertyName("upper_division_id")]
        public int? UpperDivisionId { get; set; }

        [JsonPropertyName("lower_division_id")]
        public int? LowerDivisionId { get; set; }
    }
}
