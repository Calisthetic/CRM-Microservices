using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class TimeOffInfoDto
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        public string Status { get; set; } = null!;

        public string? Description { get; set; }

        [JsonPropertyName("start")]
        public DateTime StartTimeOff { get; set; }
        [JsonPropertyName("end")]
        public DateTime EndTimeOff { get; set; }
    }
}
