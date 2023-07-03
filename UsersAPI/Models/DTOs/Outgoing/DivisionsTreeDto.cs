using Mapster;
using UsersAPI.Models.DB;
using Newtonsoft.Json;

namespace UsersAPI.Models.DTOs.Outgoing
{
    public class DivisionsTreeDto
    {
        [JsonProperty("id")]
        public int DivisionId { get; set; }

        [JsonProperty("name")]
        public string DivisionName { get; set; } = null!;
        [JsonProperty("company")]
        public string CompanyName { get; set; } = null!;
        [JsonProperty("lower_division")]
        public List<DivisionsTreeItemDto>? InverseUpperDivision { get; set; }
        public bool ShouldSerializeInverseUpperDivision()
        {
            // don't serialize the Manager property if an employee is their own manager
            return false;// (InverseUpperDivision != null && InverseUpperDivision.Count < 0);
        }
    }
}
