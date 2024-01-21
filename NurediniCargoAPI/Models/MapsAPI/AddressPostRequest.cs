using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class AddressPostRequest
    {
        [JsonPropertyName("address")]
        public required AddressRequest Address { get; set; }
    }
}
