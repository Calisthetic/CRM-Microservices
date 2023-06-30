using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class UsersTimeOff
{
    [JsonIgnore]
    public int UserTimeOffId { get; set; }
    [JsonIgnore]
    public int UserId { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    [JsonPropertyName("start")]
    public DateTime StartTimeOff { get; set; }

    [JsonPropertyName("end")]
    public DateTime EndTimeOff { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
