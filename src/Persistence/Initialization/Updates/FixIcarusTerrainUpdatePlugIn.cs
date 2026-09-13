// <copyright file="FixIcarusTerrainUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fixes the Icarus terrain and warp location.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("446A6ECF-7E63-400E-BCBC-069344387CDD")]
public sealed class FixIcarusTerrainUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Icarus Terrain";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Synchronizes Icarus terrain with the client and moves its warp arrival to walkable ground.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixIcarusTerrain;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 13, 6, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var icarus = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Icarus.Number && map.Discriminator == 0);
        icarus?.UpdateTerrainFromResources(terrainMapNumber: Icarus.Number);

        if (gameConfiguration.WarpList.FirstOrDefault(warp => warp.Index == 23)?.Gate is { } gate)
        {
            gate.X1 = 53;
            gate.Y1 = 74;
            gate.X2 = 56;
            gate.Y2 = 77;
        }

        return ValueTask.CompletedTask;
    }
}
