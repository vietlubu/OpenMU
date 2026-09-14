// <copyright file="ConfigureIllusionOfKundunSevenLootUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures guaranteed Box of Kundun and GM Gift rewards for Illusion of Kundun 7.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("861A8FCA-EBDC-4BB7-9763-E24BBE62A25F")]
public sealed class ConfigureIllusionOfKundunSevenLootUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure Illusion of Kundun 7 Loot";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Makes Illusion of Kundun 7 drop six Box of Kundun +4, six Box of Kundun +5, and six GM Gift boxes.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureIllusionOfKundunSevenLoot;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 14, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureIllusionOfKundunSevenDrops(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
