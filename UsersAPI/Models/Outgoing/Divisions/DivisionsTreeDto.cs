using Mapster;
using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.Outgoing.Divisions
{
    public class DivisionsTreeDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;
        [JsonPropertyName("company")]
        public string CompanyName { get; set; } = null!;
        [JsonPropertyName("lower_division")]
        public List<DivisionsTreeItemDto>? InverseUpperDivision { get; set; }
    }
}
