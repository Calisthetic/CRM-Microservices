using System.Text.Json.Serialization;

namespace UsersAPI.Models.Incoming.Users
{
    public class UserUpdateDto
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }
        [JsonPropertyName("second_name")]
        public string? SecondName { get; set; }
        [JsonPropertyName("third_name")]
        public string? ThirdName { get; set; }

        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("login")]
        public string? Login { get; set; }
        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("division_id")]
        public int? DivisionId { get; set; }

        [JsonPropertyName("vacation_count")]
        public short? VacationCount { get; set; }
        [JsonPropertyName("upper_user_id")]
        public int? UpperUserId { get; set; }
    }
}
