﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UsersAPI.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public int? UpperCompanyId { get; set; }

    [JsonIgnore]
    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();

    public virtual ICollection<Company> InverseUpperCompany { get; set; } = new List<Company>();

    public virtual Company? UpperCompany { get; set; }
}
