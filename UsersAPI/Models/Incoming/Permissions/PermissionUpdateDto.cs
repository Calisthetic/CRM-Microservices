﻿using System.Text.Json.Serialization;

namespace UsersAPI.Models.Incoming.Permissions
{
    public class PermissionUpdateDto
    {
        [JsonPropertyName("id")]
        public int PermissionId { get; set; }
        [JsonPropertyName("description")]
        public string PermissionDescription { get; set; } = null!;
    }
}
