using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class DivisionsTreeDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        public string Company { get; set; } = null!;
        [JsonPropertyName("lower_division")]
        public List<DivisionsTreeDto>? InverseUpperDivision { get; set; }
    }
}
