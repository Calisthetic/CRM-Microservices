﻿using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.Outgoing.Divisions
{
    public class DivisionInfoWithAddsDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        [JsonPropertyName("prefix")]
        public string? DivisionPrefix { get; set; }

        [JsonPropertyName("company")]
        public string CompanyName { get; set; } = null!;

        [JsonPropertyName("lower_division")]
        public virtual DivisionInfoDto? LowerDivision { get; set; }

        [JsonPropertyName("upper_division")]
        public virtual DivisionInfoDto? UpperDivision { get; set; }
    }
}
