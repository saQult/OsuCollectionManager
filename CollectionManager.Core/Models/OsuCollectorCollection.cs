using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManager.Core.Dto;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace CollectionManager.Core.Models
{
    public class OsuCollectorCollection
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BeatmapCount { get; set; }
        public List<Beatmap> Beatmaps { get; set; } = [];
        public List<Beatmapset> Beatmapsets { get; set; } = [];

        public static Result<OsuCollectorCollection> FromApi(OsuCollectorResponseV1? responseV1,
            OsuCollectorResponseV3? responseV3)
        {
            if(responseV1 is null || responseV3 is null)
            {
                return Result.Failure<OsuCollectorCollection>("Invalid response");
            }
            try
            {
                var collection = new OsuCollectorCollection
                {
                    Id = responseV1.Id,
                    Name = responseV1.Name,
                    Description = responseV1.Description,
                    BeatmapCount = responseV1.BeatmapCount,
                    Beatmaps = responseV3.Beatmaps,
                    Beatmapsets = responseV3.Beatmapsets
                };
                return Result.Success(collection);
            }
            catch (Exception ex)
            {
                return Result.Failure<OsuCollectorCollection>($"Failed to create collection: {ex.Message}");
            }
        }
    }
}
