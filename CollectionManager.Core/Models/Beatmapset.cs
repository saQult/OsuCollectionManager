using Newtonsoft.Json;

namespace CollectionManager.Core.Models
{
    public class Beatmapset
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;
        [JsonProperty("artist")]
        public string Artist { get; set; } = string.Empty;  
        [JsonProperty("creator")]
        public string Creator { get; set; } = string.Empty;
    }
}
