using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class Division
{
    public int DivisionId { get; set; }

    public int? UpperDivisionId { get; set; }

    public string DivisionName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
