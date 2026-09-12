// <copyright file="AddInstantServerLuckUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds guaranteed Luck to x9999 shop equipment and Box of Kundun rewards.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C6A8D3B2-8F36-40AE-8CB4-CC256F756DEE")]
public sealed class AddInstantServerLuckUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Guarantee x9999 Equipment Luck";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Adds Luck to shop equipment and guarantees Luck on equipment opened from every Box of Kundun tier.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddInstantServerLuck;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 12, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureGuaranteedLuck(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
