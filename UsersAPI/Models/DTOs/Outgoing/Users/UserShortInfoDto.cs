using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing.Users
{
    public class UserShortInfoDto
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("second_name")]
        public string SecondName { get; set; } = null!;
        [JsonPropertyName("third_name")]
        public string? ThirdName { get; set; }

        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;

        public string? Division { get; set; }
        public string Company { get; set; } = null!;

        [JsonPropertyName("vacation_count")]
        public short VacationCount { get; set; }

        [JsonPropertyName("profile_image")]
        public string? ProfileImage { get; set; }
    }
}
