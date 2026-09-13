// <copyright file="RebalanceInstantServerUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Applies the current x9999 gameplay balance to an existing Season 6 configuration.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("A33F69F2-7C5E-4A61-9B84-2D8F6E4C1057")]
public sealed class RebalanceInstantServerUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Rebalance x9999 Gameplay";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Sets all Chaos Goblin mixes to 100%, normalizes shop gear to +9/+16, strengthens x9999 bosses and Icarus, and curates full-option Kundun and GM Gift rewards.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RebalanceInstantServer;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 13, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureGameplayBalance(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
