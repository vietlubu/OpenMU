// <copyright file="SyncLorenciaTerrainUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Synchronizes the Lorencia terrain with the game client.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("608090CF-56EC-4999-97F5-5DF414B9087D")]
public class SyncLorenciaTerrainUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Sync Lorencia Terrain";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Synchronizes Lorencia's walkable terrain with the game client.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.SyncLorenciaTerrain;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 13, 5, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var lorencia = gameConfiguration.Maps.FirstOrDefault(map => map.Number == 0);
        lorencia?.UpdateTerrainFromResources();
        return ValueTask.CompletedTask;
    }
}
