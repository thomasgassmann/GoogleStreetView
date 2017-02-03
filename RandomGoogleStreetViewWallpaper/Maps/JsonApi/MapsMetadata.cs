namespace TG.Maps.JsonApi
{
    using Newtonsoft.Json;
    using System;

    public class MapsMetadata
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("location")]
        public MapsLocation Location { get; set; }

        [JsonProperty("pano_id")]
        public string PanoramaId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}