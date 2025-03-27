using Newtonsoft.Json;
using CollectionManager.Core.Models;

namespace CollectionManager.Core.Dto
{
    public record OsuCollectorResponseV3
    {
        [JsonProperty("beatmaps")]
        public List<Beatmap> Beatmaps { get; set; } = [];
        [JsonProperty("beatmapsets")]
        public List<Beatmapset> Beatmapsets { get; set; } = [];
    }
}
