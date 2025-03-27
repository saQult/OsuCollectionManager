using System.Data;
using System.Windows;
using System.Windows.Controls;
using CollectionManager.Core;
using CollectionManager.Core.Api;
using CollectionManager.Core.Models;
using CSharpFunctionalExtensions;

namespace CollectionManager
{
    public partial class OsuCollectorPage : Page
    {
        private CatboyMirror _catboyMirror = new CatboyMirror();
        private NerinyanMirror _nerinyanMirror = new NerinyanMirror();
        private BeatconnectMirror _beatconnectMirror = new BeatconnectMirror();

        public OsuCollectorPage()
        {
            InitializeComponent();
            switch (Config.Mirror)
            {
                case "Auto": MirrorSelector.SelectedIndex = 0; break;
                case "Catboy": MirrorSelector.SelectedIndex = 1; break;
                case "Nerinyan": MirrorSelector.SelectedIndex = 2; break;
                case "Beatconnect": MirrorSelector.SelectedIndex = 3; break;
            }
        }

        private async Task DownloadBeatmaps(OsuCollectorCollection collection, string destination)
        {
            Log("Started downloading");
            try
            {
                var beatmapsets = collection.Beatmapsets;

                Func<Beatmapset, Task<Result>> superDownloader = beatmapset => Task.FromResult(Result.Failure("No mirror selected"));
                int mirrorSwitchCounter = 0;
                switch (Config.Mirror)
                {
                    case "Catboy":
                        superDownloader = beatmapset => _catboyMirror
                            .DownloadBeatmap(beatmapset, Config.RetryCount, destination);
                        break;
                    case "Nerinyan":
                        superDownloader = beatmapset => _nerinyanMirror
                            .DownloadBeatmap(beatmapset, Config.RetryCount, destination);
                        break;
                    case "Beatconnect":
                        superDownloader = beatmapset => _beatconnectMirror
                            .DownloadBeatmap(beatmapset, Config.RetryCount, destination);
                        break;
                    case "Auto":
                        superDownloader = async beatmapset =>
                        {
                            switch(mirrorSwitchCounter)
                            {
                                case 0:
                                    {
                                        mirrorSwitchCounter++;
                                        Log($"[BEATCONNECT] Downloading {beatmapset.Artist} - {beatmapset.Title}");
                                        var result = await _beatconnectMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        result = await _nerinyanMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        return await _catboyMirror.DownloadBeatmap(beatmapset, 3);
                                    }
                                case 1:
                                    {
                                        mirrorSwitchCounter++;

                                        Log($"[NERINYAN] Downloading {beatmapset.Artist} - {beatmapset.Title}");
                                        var result = await _nerinyanMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        result = await _beatconnectMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        return await _catboyMirror.DownloadBeatmap(beatmapset, 3);
                                    }
                                default:
                                    {
                                        mirrorSwitchCounter = 0;

                                        Log($"[CATBOY] Downloading {beatmapset.Artist} - {beatmapset.Title}");
                                        var result = await _catboyMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        result = await _nerinyanMirror.DownloadBeatmap(beatmapset, 3);
                                        if (result.IsSuccess) return result;

                                        return await _beatconnectMirror.DownloadBeatmap(beatmapset, 3);
                                    } 
                            }

                        };
                        break;
                    default:
                        {
                            MessageBox.Show("Please select mirror");
                            return;
                        } 
                }
                int downloadCount = 0;
                int threadCount = Config.Threads > 0 ? Config.Threads : beatmapsets.Count;
                threadCount = Config.Mirror == "Auto" ? threadCount * 3 : threadCount;
                var taskBatches = beatmapsets
                    .Select((beatmap, index) => new { beatmap, index })
                    .GroupBy(x => x.index % threadCount)
                    .Select(group => Task.Run(async () =>
                    {
                        foreach (var item in group)
                        {
                            var result = await superDownloader(item.beatmap);
                            if (result.IsSuccess)
                            {
                                downloadCount++;
                                Log($"Downloaded {item.beatmap.Artist} - {item.beatmap.Title}");
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    DownloadCountInfoLabel.Content = $"Downloading {downloadCount}/{beatmapsets.Count}";
                                });
                            }
                            else
                            {
                                Log($"Failed to download {item.beatmap.Artist} - {item.beatmap.Title}: {result.Error}");
                            }
                        }
                    }))
                    .ToList();
                await Task.WhenAll(taskBatches);
                DownloadCountInfoLabel.Content = $"Downloaded finished. Downloaded {downloadCount}/{beatmapsets.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void MakeCollection(OsuCollectorCollection osuCollectorCollection)
        {
            Log($"Creating collection for {osuCollectorCollection.Name}");

            string filePath = Config.DestinationFolder
                + $"\\{osuCollectorCollection.Name}.db";

            var database = new CollectionDb();
            database.Collections.Add(
                new Collection()
                {
                    Name = osuCollectorCollection.Name,
                    Hashes = osuCollectorCollection.Beatmaps
                        .Select(x => x.Checksum).ToList(),
                });

            var saveResult = database.SaveCollections(database.FilePath);
            if (saveResult.IsFailure)
            {
                MessageBox.Show(saveResult.Error);
                return;
            }

            Log($"Created collection: {filePath}");
        }
        private void MergeCollection(OsuCollectorCollection osuCollectorCollection)
        {
            Log($"Merging collection {osuCollectorCollection.Name}");
            var database = new CollectionDb()
            {
                FilePath = Config.OsuPath + "\\collection.db"
            };
            var loadResult = database.LoadCollections();
            if (loadResult.IsFailure)
            {
                MessageBox.Show(loadResult.Error);
                return;
            }
            database.Collections.Add(
                new Collection()
                {
                    Name = osuCollectorCollection.Name,
                    Hashes = osuCollectorCollection.Beatmaps
                        .Select(x => x.Checksum).ToList(),
                });
            var saveResult = database.SaveCollections(database.FilePath);
            if (saveResult.IsFailure)
            {
                MessageBox.Show(saveResult.Error);
                return;
            }
            Log("Collection merged");
        }
        private void ChangeMirror(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem mirror = (ComboBoxItem)MirrorSelector.SelectedItem;
            if(mirror is null) return;
            if(mirror.Content is null) return;
            Config.Mirror = mirror.Content.ToString()!;
            Config.Save();
        }

        private void Log(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogTextBox.Text = message + '\n' + LogTextBox.Text;
            });
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            var collectionResult = await OsuCollectorApi.GetCollectionInfo(CollectionIdInput.Value);
            if (collectionResult.IsFailure)
            {
                MessageBox.Show(collectionResult.Error);
                return;
            }
            switch (DownloadOption.SelectedIndex)
            {
                case 0:
                    {
                        await DownloadBeatmaps(collectionResult.Value, Config.OsuPath);
                    } break;
                case 1:
                    {
                        await DownloadBeatmaps(collectionResult.Value, Config.OsuPath + "\\Songs\\");
                        MergeCollection(collectionResult.Value);
                    } break;
                case 2:
                    {
                        Log($"Parsing from osu!collector...");

                        MakeCollection(collectionResult.Value);
                    } break;
                case 3:
                    {
                        MergeCollection(collectionResult.Value);
                    } break;
                default:
                    {
                        MessageBox.Show("Select download option");
                    }
                    break;
            }
        }
    }
}
