using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class PermissionsOfDivision
{
    public int PermissionOfDivisionId { get; set; }

    public int DivisionId { get; set; }

    public int PermissionId { get; set; }
    [JsonIgnore]
    public virtual Division Division { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}
