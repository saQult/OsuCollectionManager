using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManager.Core.Dto;
using CollectionManager.Core.Models;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;

namespace CollectionManager.Core.Api
{
    public static class OsuCollectorApi
    {
        private static readonly string _endpoint = "https://osucollector.com/api/";
        public static async Task<Result<OsuCollectorCollection>> GetCollectionInfo(int collectionId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var url = $"{_endpoint}/collections/{collectionId}";
                    var response = await client.GetAsync(url);
                    var content = await response.Content.ReadAsStringAsync();
                    var responseV1 = JsonConvert.DeserializeObject<OsuCollectorResponseV1>(content);

                    url = $"{_endpoint}/collections/{collectionId}/beatmapsv3";
                    response = await client.GetAsync(url);
                    content = await response.Content.ReadAsStringAsync();
                    var responseV3 = JsonConvert.DeserializeObject<OsuCollectorResponseV3>(content);

                    return OsuCollectorCollection.FromApi(responseV1, responseV3);
                }
                catch(Exception ex)
                {
                    return Result.Failure<OsuCollectorCollection>("Failed to get collection: " + ex.Message);
                }
            }
        }
    }
}
