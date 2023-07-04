using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing.Users
{
    public class UserAuthResultDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = null!;
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = null!;
    }
}
