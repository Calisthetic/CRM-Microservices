using System.Text.Json.Serialization;
using UsersAPI.Models.Outgoing.Users;

namespace UsersAPI.Models.Outgoing
{
    public class TimeOffWithUserDto
    {
        [JsonPropertyName("id")]
        public int UserTimeOffId { get; set; }

        public string Status { get; set; } = null!;

        public string? Description { get; set; }

        [JsonPropertyName("start")]
        public DateTime StartTimeOff { get; set; }
        [JsonPropertyName("end")]
        public DateTime EndTimeOff { get; set; }

        public virtual UserShortInfoDto User { get; set; } = null!;
    }
}
