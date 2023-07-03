﻿using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class DivisionsThreeDto
    {
        [JsonPropertyName("id")]
        public int DivisionId { get; set; }

        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;

        public string Company { get; set; } = null!;
        [JsonPropertyName("upper_division")]
        public List<DivisionsThreeDto>? InverseUpperDivision { get; set; }
    }
}
