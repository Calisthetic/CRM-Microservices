using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class UsersTimeOff
{
    public int UserTimeOffId { get; set; }

    public int UserId { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartTimeOff { get; set; }

    public DateTime EndTimeOff { get; set; }

    public virtual User User { get; set; } = null!;
}
