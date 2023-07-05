using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Incoming
{
    public class PermissionAddNew
    {
        [JsonPropertyName("name")]
        public string PermissionName { get; set; } = null!;
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
