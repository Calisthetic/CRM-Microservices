using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing.Divisions
{
    public class DivisionsTreeItemDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;
        [JsonPropertyName("lower_division")]
        public List<DivisionsTreeItemDto>? InverseUpperDivision { get; set; }
    }
}
