using CollectionManager.Core.Models;
using CSharpFunctionalExtensions;

namespace CollectionManager.Core.Api
{
    public abstract class Mirror()
    {
        public virtual event EventHandler? FailedDownloading;
        public virtual event EventHandler? StartedDownloading;
        public virtual event EventHandler? SuccessDownloading;

        public virtual async Task<Result> DownloadBeatmap(Beatmapset beatmapset, int retryCount, string destinationFolder)
        {
            await Task.Delay(0);
            return Result.Success();
        }
    }
}