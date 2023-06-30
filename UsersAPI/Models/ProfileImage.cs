using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class ProfileImage
{
    public int ProfileImageId { get; set; }

    public int UserId { get; set; }

    public int ImageId { get; set; }

    public virtual User User { get; set; } = null!;
}
