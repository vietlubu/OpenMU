// <copyright file="IncreaseInstantServerMoneyDropUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Raises the Zen reward of the Season 6 instant-server configuration.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C5B129C9-320E-486D-B21A-638515B53976")]
public sealed class IncreaseInstantServerMoneyDropUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Increase x9999 Zen Drops";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Multiplies picked-up Zen by 1000 for the x9999 instant server.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.IncreaseInstantServerMoneyDrop;

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
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureMoneyAmountRate(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
