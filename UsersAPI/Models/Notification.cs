using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string NotificationText { get; set; } = null!;

    public string? ActionLink { get; set; }

    public bool Delivered { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
