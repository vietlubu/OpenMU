// <copyright file="ReduceKalimaSevenRegularMonsterStrengthUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Reduces the strength of Kalima 7's regular monsters.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("3F21AF06-3C64-497F-BE8C-AC0EB28D0E35")]
public sealed class ReduceKalimaSevenRegularMonsterStrengthUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Reduce Kalima 7 Regular Monster Strength";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Reduces Kalima 7 regular monster attributes by a further 30 percent without changing Illusion of Kundun 7.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ReduceKalimaSevenRegularMonsterStrength;

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
        ConfigureKalimaSevenUpdatePlugIn.ApplyRegularMonsterStats(gameConfiguration, 1.47f);
        return ValueTask.CompletedTask;
    }
}
