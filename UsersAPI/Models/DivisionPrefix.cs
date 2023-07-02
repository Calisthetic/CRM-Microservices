using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class DivisionPrefix
{
    public int DivisionPrefixId { get; set; }

    public int? UpperDivisionPrefixId { get; set; }

    public string DivisionPrefixName { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();
    [JsonIgnore]
    public virtual ICollection<DivisionPrefix> InverseUpperDivisionPrefix { get; set; } = new List<DivisionPrefix>();
    [JsonIgnore]
    public virtual DivisionPrefix? UpperDivisionPrefix { get; set; }
}
