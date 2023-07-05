using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Incoming.Divisions
{
    public class DivisionUpdatePermissionsDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("permissions")]
        public int[] PermissionIds { get; set; } = null!;
    }
}
