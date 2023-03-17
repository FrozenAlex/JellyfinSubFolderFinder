using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Persistence;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.SubfolderFinder.Tasks
{
    #pragma warning disable 1591
    public class PostScanTask : ILibraryPostScanTask
    {
        private readonly ILogger<Plugin> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostScanTask"/> class.
        /// </summary>
        public PostScanTask()
        {
            _logger = Plugin.Instance?.GetLogger() ?? throw new ArgumentNullException(nameof(_logger));
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc/>
        public async Task Run(IProgress<double> progress, CancellationToken cancellationToken)
        {
            // Delay for no reason
            await Task.Delay(10, cancellationToken).ConfigureAwait(false);

            // Query all episodes
            var query = new InternalItemsQuery()
            {
                IncludeItemTypes = new[] { Jellyfin.Data.Enums.BaseItemKind.Episode },
                Recursive = true,
            };

            // Get all episodes
            var episodesList = Plugin.Instance?.GetLibraryManager().GetItemList(query);
            if (episodesList == null)
            {
                return;
            }

            // loop over series
            foreach (Episode episode in episodesList)
            {
                var folderPath = Path.GetDirectoryName(episode.Path);
                if (folderPath == null)
                {
                    continue;
                }

                // Get episode filename without extension
                var episodeFileName = Path.GetFileNameWithoutExtension(episode.Path);
                if (episodeFileName == null)
                {
                    continue;
                }

                // Search for subtitle files in subfolders
                var subtitleFiles = Directory.EnumerateFiles(folderPath, episodeFileName + ".srt", SearchOption.AllDirectories);
                
                // Add a new subtitle track for each subtitle file
                foreach (var subtitleFile in subtitleFiles)
                {
                

                    foreach (var Subtitle in episode.SubtitleFiles)
                    {
                        if (Subtitle == subtitleFile)
                        {
                            _logger.LogInformation("Subtitle already exists: {0}", subtitleFile);
                            continue;
                        }
                    }
                    _logger.LogInformation("Adding subtitle: {0}", subtitleFile);

                    string[] lastSubFiles = episode.SubtitleFiles;
                    Array.Resize(ref lastSubFiles, lastSubFiles.Length + 1);
                    lastSubFiles[lastSubFiles.Length] = subtitleFile;
                    episode.SubtitleFiles = lastSubFiles;
                }
                
                foreach (var subtitleFile in subtitleFiles)
                {
                    _logger.LogInformation("Subtitle: {0}", subtitleFile);
                }
                _logger.LogInformation("Episode: {0}", episode.Path);
            }
        }

        /// <inheritdoc/>
        public override string? ToString()
        {
            return base.ToString();
        }
    }
}
