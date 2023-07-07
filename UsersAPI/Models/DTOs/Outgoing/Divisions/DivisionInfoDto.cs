using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing.Divisions
{
    public class DivisionInfoDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        [JsonPropertyName("prefix")]
        public string DivisionPrefix { get; set; } = null!;
    }
}
