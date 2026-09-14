// <copyright file="RetuneKalimaSevenRegularMonstersUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Retunes Kalima 7's regular monster difficulty and rewards.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("A136E85E-4BE4-4661-928C-BC5DA352681B")]
public sealed class RetuneKalimaSevenRegularMonstersUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Retune Kalima 7 Regular Monsters";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Reduces Kalima 7 regular monster attributes by 30 percent and increases their Box of Kundun +4 drop chance to 20 percent.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RetuneKalimaSevenRegularMonsters;

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
        ConfigureKalimaSevenUpdatePlugIn.ApplyRegularMonsterStats(gameConfiguration, 2.1f);
        var boxFourGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(9_999, Kalima7.Number, 1));
        boxFourGroup.Chance = 0.2;
        return ValueTask.CompletedTask;
    }
}
