using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing.Users
{
    public class UpperUsersListDto
    {
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("second_name")]
        public string SecondName { get; set; } = null!;
        [JsonPropertyName("third_name")]
        public string? ThirdName { get; set; }

        public string? Division { get; set; }
        [JsonPropertyName("division_prefix")]
        public string? DivisionPrefix { get; set; }
    }
}
