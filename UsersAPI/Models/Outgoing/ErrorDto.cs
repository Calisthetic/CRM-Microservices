﻿using System.Text.Json.Serialization;

namespace UsersAPI.Models.Outgoing
{
    public class ErrorDto
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = null!;
        [JsonPropertyName("exception")]
        public string? Exception { get; set; }
    }
}
