using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class PlacesResponse
    {
        [JsonPropertyName("places")]
        public List<Place>? Places { get; set; }

        public class Place
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("formattedAddress")]
            public string? FormattedAddress { get; set; }

            [JsonPropertyName("location")]
            public LocationObj? Location { get; set; }
            public class LocationObj
            {
                [JsonPropertyName("latitude")]
                public double Latitude { get; set; }

                [JsonPropertyName("longitude")]
                public double Longitude { get; set; }
            }


            [JsonPropertyName("displayName")]
            public DisplayNameObj? DisplayName { get; set; }
            public class DisplayNameObj
            {
                [JsonPropertyName("text")]
                public string? Text { get; set; }

                [JsonPropertyName("languageCode")]
                public string? LanguageCode { get; set; }
            }
        }
    }
}
