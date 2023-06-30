using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class LikeType
{
    public int LikeTypeId { get; set; }

    public string LikeTypeName { get; set; } = null!;

    public int ImageId { get; set; }

    public virtual ICollection<UsersLike> UsersLikes { get; set; } = new List<UsersLike>();
}
