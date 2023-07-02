using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class UpperUserInfoDto
    {
        public UpperUserInfoDto(User user)
        {
            FirstName = user.FirstName;
            SecondName = user.SecondName;
            ThirdName = user.ThirdName;
            Division = user.Division?.DivisionName;
            DivisionPrefix = user.Division?.DivisionPrefix?.DivisionPrefixName;
            UpperUser = Converter(user.UpperUser);
        }

        public static UpperUserInfoDto? Converter(User? user)
        {
            return user == null ? null : new UpperUserInfoDto(user);
        }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;
        [JsonPropertyName("second_name")]
        public string SecondName { get; set; } = null!;
        [JsonPropertyName("third_name")]
        public string? ThirdName { get; set; }

        public string? Division { get; set; }
        [JsonPropertyName("division_prefix")]
        public string? DivisionPrefix { get; set; }

        [JsonPropertyName("upper_user")]
        public UpperUserInfoDto? UpperUser { get; set; }
    }
}
