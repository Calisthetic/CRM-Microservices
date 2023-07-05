using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class PermissionDto
    {
        [JsonPropertyName("id")]
        public int PermissionId { get; set; }
        [JsonPropertyName("name")]
        public string PermissionName { get; set; } = null!;
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
