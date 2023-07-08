using System.Text.Json.Serialization;

namespace UsersAPI.Models.DTOs.Incoming.Companies
{
    public class CompanyAddNewDto
    {
        [JsonPropertyName("name")]
        public string CompanyName { get; set; } = null!;

        [JsonPropertyName("upper_company_id")]
        public int? UpperCompanyId { get; set; }

        [JsonPropertyName("lower_company_id")]
        public int? LowerCompanyId { get; set; }
    }
}
