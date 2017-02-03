namespace TG.Maps.JsonApi
{
    using Newtonsoft.Json;

    public class MapsLocation
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}
