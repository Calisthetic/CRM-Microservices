using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Incoming.Permissions
{
    public class PermissionAddDto
    {
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
