using Newtonsoft.Json;

namespace CollectionManager.Core.Dto
{
    public record OsuCollectorResponseV1
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("description")]
        public string Description { get; set; } = string.Empty;
        [JsonProperty("beatmapCount")]
        public int BeatmapCount { get; set; }
    }
}
