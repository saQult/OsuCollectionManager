using Newtonsoft.Json;

namespace CollectionManager.Core.Models
{
    public class Beatmap
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("checksum")]
        public string Checksum { get; set; } = string.Empty;
    }
}
