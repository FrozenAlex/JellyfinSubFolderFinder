using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.SubfolderFinder.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.SubfolderFinder;

/// <summary>
/// The main plugin.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Gets the plugin configuration.
    /// </summary>
    private readonly ILibraryManager _libraryManager;

    /// <summary>
    /// Gets the logger.
    /// </summary>
    private readonly ILogger<Plugin> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Instance of the <see cref="IApplicationPaths"/> interface.</param>
    /// <param name="xmlSerializer">Instance of the <see cref="IXmlSerializer"/> interface.</param>
    /// <param name="libraryManager">Instance of the <see cref="ILibraryManager"/> interface.</param>
    /// <param name="logger">Instance of the <see cref="ILogger"/> interface.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILibraryManager libraryManager, ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        this._libraryManager = libraryManager;
        this._logger = logger;
    }

    /// <inheritdoc />
    public override string Name => "SubfolderFinder";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("685b7da7-1091-49f9-bfd3-0d703b005490");

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <summary>
    /// Gets the library manager.
    /// <returns>Instance of the <see cref="ILibraryManager"/> interface.</returns>
    /// </summary>
    public ILibraryManager GetLibraryManager()
    {
        return this._libraryManager;
    }

    /// <summary>
    /// Gets the logger.
    /// <returns>Instance of the <see cref="ILogger"/> interface.</returns>
    /// </summary>
    public ILogger<Plugin> GetLogger()
    {
        return this._logger;
    }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        };
    }
}
