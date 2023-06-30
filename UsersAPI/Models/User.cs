using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string? ThirdName { get; set; }

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int DivisionId { get; set; }

    public int CompanyId { get; set; }

    public int RoleId { get; set; }

    public short VacationCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Division Division { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ProfileImage> ProfileImages { get; set; } = new List<ProfileImage>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<UsersLike> UsersLikeCreatorUsers { get; set; } = new List<UsersLike>();

    public virtual ICollection<UsersLike> UsersLikeLikedUsers { get; set; } = new List<UsersLike>();

    public virtual ICollection<UsersTimeOff> UsersTimeOffs { get; set; } = new List<UsersTimeOff>();
}
