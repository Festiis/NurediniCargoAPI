using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class AddressRequest
    {
        [JsonPropertyName("regionCode")]
        public string RegionCode { get; set; } = "SE";

        [JsonPropertyName("addressLines")]
        public required List<string> AddressLines {  get; set; }
    }
}
