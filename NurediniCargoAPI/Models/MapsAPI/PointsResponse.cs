using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class PointsResponse
    {
        [JsonPropertyName("routes")]
        public required List<Route> Routes { get; set; }

        public class Route
        {
            [JsonPropertyName("legs")]
            public required List<Leg> Legs { get; set; }
        }
        public class Leg
        {
            [JsonPropertyName("localizedValues")]
            public required LocalizedValues LocalizedValues { get; set; }
        }

        public class LocalizedValues
        {
            [JsonPropertyName("distance")]
            public required LocalizedProp Distance { get; set; }

            [JsonPropertyName("duration")]
            public required LocalizedProp Duration { get; set; }

            // You can keep the StaticDuration property if needed

            // Optionally, you can add a custom property without the 'text' key
            [JsonIgnore]
            public string CleanDistance => Distance.Text;
            [JsonIgnore]
            public string CleanDuration => Duration.Text;
        }


        public class LocalizedProp
        {
            [JsonPropertyName("text")]
            public required string Text { get; set; }
        }
    }
}
