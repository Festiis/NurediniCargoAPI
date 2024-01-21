using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class PointsRequest
    {
        [JsonPropertyName("origin")]
        public required LatLng Origin {  get; set; }

        [JsonPropertyName("destination")]
        public required LatLng Destination { get; set; }
    }
}
