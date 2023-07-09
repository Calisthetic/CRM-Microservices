using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.Incoming
{
    public class TimeOffAddUpdateDto
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        public string Status { get; set; } = null!;

        public string? Description { get; set; }

        [JsonPropertyName("start")]
        public string StartTimeOff { get; set; } = null!;

        [JsonPropertyName("end")]
        public string EndTimeOff { get; set; } = null!;
    }
}
