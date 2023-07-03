using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing.Divisions
{
    public class DivisionInfoWithAddsDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        [JsonPropertyName("prefix")]
        public string? DivisionPrefix { get; set; }

        [JsonPropertyName("company")]
        public string CompanyName { get; set; } = null!;

        [JsonPropertyName("lower_division")]
        public virtual DivisionAddInfoDto? LowerDivision { get; set; }

        [JsonPropertyName("upper_division")]
        public virtual DivisionAddInfoDto? UpperDivision { get; set; }
    }
}
