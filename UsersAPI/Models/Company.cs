using System;
using System.Collections.Generic;

namespace UsersAPI.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
