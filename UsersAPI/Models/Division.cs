using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class Division
{
    public int DivisionId { get; set; }
    [JsonIgnore]
    public int? UpperDivisionId { get; set; }

    public string DivisionName { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Division> InverseUpperDivision { get; set; } = new List<Division>();

    public virtual Division? UpperDivision { get; set; }
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
