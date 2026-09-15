// <copyright file="IncreaseKalimaSevenBoxDropsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Increases Kalima 7 regular monster drop rates for Box of Kundun +4 and +5.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("6FDAAA17-15DD-4EF0-A320-73DB97197226")]
public sealed class IncreaseKalimaSevenBoxDropsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Increase Kalima 7 Box Drops";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Increases Box of Kundun +4 drop chance to 50% and adds Box of Kundun +5 with a 40% chance for Kalima 7 regular monsters.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.IncreaseKalimaSevenBoxDrops;

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
        var boxOfKundun = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 11 });
        var boxFourGroupId = GuidHelper.CreateGuid<DropItemGroup>(9_999, 36, 1);
        var boxFourGroup = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == boxFourGroupId);
        if (boxFourGroup is not null)
        {
            boxFourGroup.Chance = 0.5;
        }

        var boxFiveGroupId = GuidHelper.CreateGuid<DropItemGroup>(9_999, 36, 2);
        var boxFiveGroup = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == boxFiveGroupId);
        if (boxFiveGroup is null)
        {
            boxFiveGroup = context.CreateNew<DropItemGroup>();
            boxFiveGroup.SetGuid(boxFiveGroupId);
            gameConfiguration.DropItemGroups.Add(boxFiveGroup);
        }

        boxFiveGroup.Description = "Kalima 7 regular monster: Box of Kundun +5";
        boxFiveGroup.Chance = 0.4;
        boxFiveGroup.ItemType = SpecialItemType.RandomItem;
        boxFiveGroup.ItemLevel = 12;
        boxFiveGroup.MinimumMonsterLevel = null;
        boxFiveGroup.MaximumMonsterLevel = null;
        boxFiveGroup.Monster = null;
        boxFiveGroup.PossibleItems.Clear();
        boxFiveGroup.PossibleItems.Add(boxOfKundun);

        var regularNumbers = ConfigureKalimaSevenUpdatePlugIn.RegularMonsterStats.Select(stats => stats.Number).ToHashSet();
        foreach (var monster in gameConfiguration.Monsters.Where(monster => regularNumbers.Contains(monster.Number)))
        {
            if (!monster.DropItemGroups.Contains(boxFiveGroup))
            {
                monster.DropItemGroups.Add(boxFiveGroup);
            }
        }

        return ValueTask.CompletedTask;
    }
}
