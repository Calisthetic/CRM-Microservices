using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models.DB;

public partial class Division
{
    public int DivisionId { get; set; }

    public int? UpperDivisionId { get; set; }

    public string DivisionName { get; set; } = null!;

    public int CompanyId { get; set; }

    public int? DivisionPrefixId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual DivisionPrefix? DivisionPrefix { get; set; }
    [JsonIgnore]
    public virtual ICollection<Division> InverseUpperDivision { get; set; } = new List<Division>();

    public virtual ICollection<PermissionsOfDivision> PermissionsOfDivisions { get; set; } = new List<PermissionsOfDivision>();
    public virtual Division? UpperDivision { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
