using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Net.Http.Headers;
using NurediniCargoAPI.Models.MapsAPI;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using static NurediniCargoAPI.Models.MapsAPI.PointsResponse;

namespace NurediniCargoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : ControllerBase
    {

        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
        private readonly IConfiguration _configuration = configuration;
        private readonly JsonSerializerOptions webOptions = new(JsonSerializerDefaults.Web);

        [HttpPost("/DistanceBetween")]
        public async Task<ActionResult> DistnaceBetweenPoints([FromBody] PointsRequest points)
        {
            if (points == null || points.Destination.Latitude == 0 || points.Destination.Longitude == 0 || points.Origin.Latitude == 0 || points.Origin.Longitude == 0)
            {
                return BadRequest();
            }

            var apiKey = _configuration["GoogleAPIKey"];

            if (string.IsNullOrEmpty(apiKey) || apiKey == null)
            {
                return StatusCode(500);
            }

            string apiUrl = $"https://routes.googleapis.com/directions/v2:computeRoutes";

            using StringContent json = new(
            JsonSerializer.Serialize(
                new PointsPostRequest(points),
                webOptions
            ),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

            HttpRequestMessage request = new(HttpMethod.Post, apiUrl)
            {
                Content = json
            };
            request.Headers.Add("X-Goog-Api-Key", apiKey);
            request.Headers.Add("X-Goog-FieldMask", "routes.legs.localizedValues.distance,routes.legs.localizedValues.duration");

            using HttpResponseMessage httpResponse = await _httpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();


            var pointsResponse = JsonSerializer.Deserialize<PointsResponse>(await httpResponse.Content.ReadAsStringAsync());

            // Access the clean duration
            if (pointsResponse != null && pointsResponse?.Routes?.Count > 0)
            {

                return Ok(JsonSerializer.Serialize(
                    new PointsResult
                    {
                        Duration = pointsResponse.Routes[0].Legs[0].LocalizedValues.CleanDuration,
                        Distance = pointsResponse.Routes[0].Legs[0].LocalizedValues.CleanDistance
                    },
                    webOptions
                ));

            }
             

            return BadRequest();

        }

        [HttpPost("/ValidateAddress")]
        public async Task<ActionResult> ValidateAddress([FromBody] AddressRequest address)
        {

            if (address == null || address.AddressLines.Count == 0)
            {
                return BadRequest();
            }

            var apiKey = _configuration["GoogleAPIKey"];

            if (string.IsNullOrEmpty(apiKey) || apiKey == null)
            {
                return StatusCode(500);
            }
            string apiUrl = $"https://addressvalidation.googleapis.com/v1:validateAddress?key={apiKey}";


            using StringContent json = new(
                JsonSerializer.Serialize(
                    new AddressPostRequest
                    {
                        Address = address
                    },
                    webOptions
                ),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            // Create a HttpRequestMessage and set headers
            HttpRequestMessage request = new(HttpMethod.Post, apiUrl)
            {
                Content = json
            };

            using HttpResponseMessage httpResponse = await _httpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();


            var addressResponse = JsonSerializer.Deserialize<AddressResponse>(await httpResponse.Content.ReadAsStringAsync());


            if (addressResponse?.IsAddressComplete() == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Invalid or incomplete address data.");
            }



        }

        [HttpPost("/ValidatePlace/{address}")]
        public async Task<ActionResult> ValidatePlace(string address)
        {

            if (address == null)
            {
                return BadRequest();
            }

            string apiUrl = "https://places.googleapis.com/v1/places:searchText";
            var apiKey = _configuration["GoogleAPIKey"]; 

            if (string.IsNullOrEmpty(apiKey) || apiKey == null)
            {
                return StatusCode(500);
            }

            using StringContent json = new(
                JsonSerializer.Serialize(
                    new {
                        textQuery = address,
                        languageCode = "sv",
                    },
                    webOptions
                ),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);

            // Create a HttpRequestMessage and set headers
            HttpRequestMessage request = new(HttpMethod.Post, apiUrl)
            {
                Content = json
            };
            request.Headers.Add("X-Goog-Api-Key", apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.id,places.displayName,places.formattedAddress,places.location");

            using HttpResponseMessage httpResponse = await _httpClient.SendAsync(request);

            httpResponse.EnsureSuccessStatusCode();

            var placesResponse = JsonSerializer.Deserialize<PlacesResponse>(await httpResponse.Content.ReadAsStringAsync());

            var place = placesResponse?.Places?.Count == 1 ? placesResponse.Places[0] : null;

            if (place != null)
            {
                return Ok(JsonSerializer.Serialize(place));
            }

            return NotFound();
        }
    }
}
