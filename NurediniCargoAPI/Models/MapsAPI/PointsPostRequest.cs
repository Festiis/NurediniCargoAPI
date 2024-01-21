using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class PointsPostRequest(PointsRequest points)
    {
        [JsonPropertyName("origin")]
        public Point Origin { get; set; } = new Point(points.Origin);

        [JsonPropertyName("destination")]
        public Point Destination { get; set; } = new Point(points.Destination);

        [JsonPropertyName("travelMode")]
        public string TravelMode { get; set; } = "DRIVE";

        [JsonPropertyName("routingPreference")]
        public string RoutingPreference { get; set; } = "TRAFFIC_AWARE";

        [JsonPropertyName("computeAlternativeRoutes")]
        public bool ComputeAlternativeRoutes { get; set; } = false;

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = "sv-SE";

        [JsonPropertyName("units")]
        public string Units { get; set; } = "METRIC";

        public class Point
        {
            [JsonPropertyName("location")]
            public Location Location { get; set; }

            public Point(LatLng latLng)
            {
                Location = new Location { LatLng = latLng };
            }
        }

        public class Location 
        {
            [JsonPropertyName("latLng")]
            public required LatLng LatLng { get; set; }
        }
    }
}