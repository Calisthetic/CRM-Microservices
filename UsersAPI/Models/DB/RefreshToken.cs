using System;
using System.Collections.Generic;

namespace UsersAPI.Models.DB;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public string JwtId { get; set; } = null!;

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public virtual User User { get; set; } = null!;
}
