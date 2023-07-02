using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models.DB;

public partial class UsersLike
{
    public int UserLikeId { get; set; }

    public int LikedUserId { get; set; }

    public int CreatorUserId { get; set; }

    public int LikeTypeId { get; set; }

    public virtual User CreatorUser { get; set; } = null!;

    public virtual LikeType LikeType { get; set; } = null!;

    [JsonIgnore]
    public virtual User LikedUser { get; set; } = null!;
}
