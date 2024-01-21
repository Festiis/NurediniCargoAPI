using System.Text.Json.Serialization;

namespace NurediniCargoAPI.Models.MapsAPI
{
    public class AddressResponse
    {
        [JsonPropertyName("verdict")]
        public VerdictClass? Verdict { get; set; }

        public class VerdictClass
        {
            [JsonPropertyName("inputGranularity")]
            public string? InputGranularity { get; set; }

            [JsonPropertyName("validationGranularity")]
            public string? ValidationGranularity { get; set; }

            [JsonPropertyName("geocodeGranularity")]
            public string? GeocodeGranularity { get; set; }

            [JsonPropertyName("addressComplete")]
            public bool? AddressComplete { get; set; }
        }

        // Method to check if address is complete based on the verdict
        public bool IsAddressComplete()
        {
            return Verdict?.AddressComplete ?? false;
        }
    }
}
