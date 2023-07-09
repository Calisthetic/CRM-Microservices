using System.Text.Json.Serialization;

namespace UsersAPI.Models.Incoming.Permissions
{
    public class PermissionAddDto
    {
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
