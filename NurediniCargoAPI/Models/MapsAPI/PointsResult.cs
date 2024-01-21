using static NurediniCargoAPI.Models.MapsAPI.PointsResponse;
using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class PointsResult
    {
        [JsonPropertyName("distance")]
        public string? Distance { get; set; }

        [JsonPropertyName("duration")]
        public string? Duration { get; set; }
    }
}
