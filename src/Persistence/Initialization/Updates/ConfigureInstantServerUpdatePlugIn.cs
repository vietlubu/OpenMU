// <copyright file="ConfigureInstantServerUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Converts an existing Season 6 configuration into the high-rate instant-server realm.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("0A592464-46CA-4F5B-B7D9-E957347AD2B8")]
public sealed class ConfigureInstantServerUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure x9999 Instant Server";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Sets x9999 experience, extreme drops, 500 points per level, dense monster packs, full PvP, broad NPC shops, and continuous boss invasions.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureInstantServer;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 11, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.Apply(context, gameConfiguration);
        InstantServerConfiguration.ConfigureBossEvents(gameConfiguration);

        foreach (var server in await context.GetAsync<GameServerDefinition>().ConfigureAwait(false))
        {
            server.ExperienceRate = 1.0f;
            server.PvpEnabled = true;
        }
    }
}
