// <copyright file="IncreaseKalimaSevenDifficultyUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Triples the attributes of Kalima 7's regular monsters.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("69DA0CAC-B9D7-47B1-B5DE-3D3FB899720A")]
public sealed class IncreaseKalimaSevenDifficultyUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Increase Kalima 7 Difficulty";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Triples Kalima 7 regular monster health, damage, defense, attack rate, and defense rate without changing Illusion of Kundun 7.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.IncreaseKalimaSevenDifficulty;

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
        ConfigureKalimaSevenUpdatePlugIn.ApplyRegularMonsterStats(gameConfiguration, 3);
        return ValueTask.CompletedTask;
    }
}
