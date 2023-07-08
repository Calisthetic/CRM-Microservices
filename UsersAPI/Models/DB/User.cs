using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models.DB;

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

    public int? DivisionId { get; set; }

    public short VacationCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UpperUserId { get; set; }
    [JsonIgnore]
    public virtual Division? Division { get; set; }

    [JsonIgnore]
    public virtual ICollection<User> InverseUpperUser { get; set; } = new List<User>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ProfileImage> ProfileImages { get; set; } = new List<ProfileImage>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual User? UpperUser { get; set; }

    public virtual ICollection<UsersLike> UsersLikeCreatorUsers { get; set; } = new List<UsersLike>();

    public virtual ICollection<UsersLike> UsersLikeLikedUsers { get; set; } = new List<UsersLike>();

    public virtual ICollection<UsersTimeOff> UsersTimeOffs { get; set; } = new List<UsersTimeOff>();
}
