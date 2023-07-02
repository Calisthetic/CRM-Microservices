using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models.DB;

public partial class Permission
{
    public int PermissionId { get; set; }

    public string? PermissionName { get; set; }
    [JsonIgnore]
    public virtual ICollection<PermissionsOfDivision> PermissionsOfDivisions { get; set; } = new List<PermissionsOfDivision>();
}
