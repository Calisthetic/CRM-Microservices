﻿using System.Text.Json.Serialization;

namespace UsersAPI.Models.Outgoing
{
    public class PermissionInfoDto
    {
        [JsonPropertyName("id")]
        public int PermissionId { get; set; }
        [JsonPropertyName("name")]
        public string PermissionName { get; set; } = null!;
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
