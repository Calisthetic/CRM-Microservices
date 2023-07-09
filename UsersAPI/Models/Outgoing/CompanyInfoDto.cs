using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class CompanyInfoDto
    {
        [JsonPropertyName("id")]
        public int CompanyId { get; set; }

        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;
    }
}
