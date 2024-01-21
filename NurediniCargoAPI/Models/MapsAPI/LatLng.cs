using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class LatLng
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
}
