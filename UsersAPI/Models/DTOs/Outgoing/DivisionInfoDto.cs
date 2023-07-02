using System.Text.Json.Serialization;
using UsersAPI.Models.DB;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class DivisionInfoDto
    {
        public DivisionInfoDto(Division division) {
            DivisionName = division.DivisionName;
            UpperDivision = Converter(division.UpperDivision);
        }
        private DivisionInfoDto? Converter(Division? division)
        {
            return division == null ? null : new DivisionInfoDto(division) { DivisionName = division.DivisionName, UpperDivision = Converter(division.UpperDivision) };
        }
        [JsonPropertyName("name")]
        public string DivisionName { get; set; } = null!;
        [JsonPropertyName("upper_division")]
        public DivisionInfoDto? UpperDivision { get; set; }
    }
}
