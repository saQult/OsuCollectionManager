using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionManager.Core.Models;
using CSharpFunctionalExtensions;

namespace CollectionManager.Core.Api
{
    public class NerinyanMirror : Mirror
    {
        private static readonly string _endpoint = "https://api.nerinyan.moe";
        public override event EventHandler? StartedDownloading;
        public override event EventHandler? FailedDownloading;
        public override event EventHandler? SuccessDownloading;
        public override async Task<Result> DownloadBeatmap(Beatmapset beatmapset, int retryCount, string destinationFolder = "")
        {
            StartedDownloading?.Invoke(beatmapset, new EventArgs());

            destinationFolder = destinationFolder ?? Config.DestinationFolder;

            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_endpoint}/d/{beatmapset.Id}"),
                };
                string customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
                client.DefaultRequestHeaders.Add("User-Agent", customUserAgent);
                using (var response = await client.SendAsync(request))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        char[] blacklist = ['\\', '/', ':', '*', '?', '"', '<', '>', '|', '+'];
                        foreach (var item in blacklist)
                        {
                            beatmapset.Title = beatmapset.Title.Replace(item, ' ');
                            beatmapset.Artist = beatmapset.Artist.Replace(item, ' ');
                        }
                        var file = $"{destinationFolder}\\{beatmapset.Artist} - {beatmapset.Title}.osz";
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                               fileStream = new FileStream(file, FileMode.Create))
                        {
                            SuccessDownloading?.Invoke(beatmapset, new EventArgs());
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (retryCount > 0)
                        {
                            FailedDownloading?.Invoke(beatmapset, new EventArgs());
                            return await DownloadBeatmap(beatmapset, retryCount - 1);
                        }
                        return Result.Failure("Failed to download beatmap: " + ex.Message);
                    }
                }
            }
            return Result.Success();
        }
    }
}
