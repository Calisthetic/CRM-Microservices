﻿using System.Text.Json.Serialization;

namespace UsersAPI.Models.Incoming.Users
{
    public class UserAddNewDto
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("second_name")]
        public string SecondName { get; set; } = null!;
        [JsonPropertyName("third_name")]
        public string? ThirdName { get; set; }

        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("login")]
        public string Login { get; set; } = null!;
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;

        [JsonPropertyName("division_id")]
        public int? DivisionId { get; set; }

        [JsonPropertyName("vacation_count")]
        public short? VacationCount { get; set; }
        [JsonPropertyName("upper_user_id")]
        public int? UpperUserId { get; set; }
    }
}
