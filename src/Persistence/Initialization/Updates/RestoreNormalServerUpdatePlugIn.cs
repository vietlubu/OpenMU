// <copyright file="RestoreNormalServerUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version095d.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Restores normal PvP rules and the standard Icarus map configuration.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("4B71DB7B-401A-49C5-851F-EABDD248809A")]
public sealed class RestoreNormalServerUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Restore Normal Server Mode";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Disables open PvP and restores the standard Icarus terrain and warp arrival.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RestoreNormalServer;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 13, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var icarus = gameConfiguration.Maps.FirstOrDefault(map => map.Number == Icarus.Number && map.Discriminator == 0);
        icarus?.UpdateTerrainFromResources();

        if (gameConfiguration.WarpList.FirstOrDefault(warp => warp.Index == 23)?.Gate is { } gate)
        {
            gate.X1 = 14;
            gate.Y1 = 13;
            gate.X2 = 16;
            gate.Y2 = 13;
        }

        foreach (var server in await context.GetAsync<GameServerDefinition>().ConfigureAwait(false))
        {
            server.PvpEnabled = false;
        }
    }
}
