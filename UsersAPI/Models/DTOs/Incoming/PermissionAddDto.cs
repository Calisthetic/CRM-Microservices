using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Incoming
{
    public class PermissionAddDto
    {
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
