// <copyright file="ConfigureKalimaSevenRegularDropsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the regular monster drops of Kalima 7.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("E24C2CF2-3C58-4CD1-876A-2E3AF56EF50F")]
public sealed class ConfigureKalimaSevenRegularDropsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure Kalima 7 Regular Drops";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Removes Zen and Box of Kundun +1 through +3 drops from Kalima 7 regular monsters and adds a 10% Box of Kundun +4 drop.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureKalimaSevenRegularDrops;

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
        var map = gameConfiguration.Maps.Single(map => map is { Number: Kalima7.Number, Discriminator: 0 });
        var moneyGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(1));
        map.DropItemGroups.Remove(moneyGroup);

        var boxOfKundun = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 11 });
        var boxFourGroupId = GuidHelper.CreateGuid<DropItemGroup>(9_999, Kalima7.Number, 1);
        var boxFourGroup = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == boxFourGroupId);
        if (boxFourGroup is null)
        {
            boxFourGroup = context.CreateNew<DropItemGroup>();
            boxFourGroup.SetGuid(boxFourGroupId);
            gameConfiguration.DropItemGroups.Add(boxFourGroup);
        }

        boxFourGroup.Description = "Kalima 7 regular monster: Box of Kundun +4";
        boxFourGroup.Chance = 0.1;
        boxFourGroup.ItemType = SpecialItemType.RandomItem;
        boxFourGroup.ItemLevel = 11;
        boxFourGroup.MinimumMonsterLevel = null;
        boxFourGroup.MaximumMonsterLevel = null;
        boxFourGroup.Monster = null;
        boxFourGroup.PossibleItems.Clear();
        boxFourGroup.PossibleItems.Add(boxOfKundun);

        var obsoleteGroups = Enumerable.Range(1, 3)
            .Select(tier => gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(9_999, (short)tier)))
            .ToList();
        var regularNumbers = ConfigureKalimaSevenUpdatePlugIn.RegularMonsterStats.Select(stats => stats.Number).ToHashSet();
        foreach (var monster in gameConfiguration.Monsters.Where(monster => regularNumbers.Contains(monster.Number)))
        {
            foreach (var group in obsoleteGroups)
            {
                monster.DropItemGroups.Remove(group);
            }

            if (!monster.DropItemGroups.Contains(boxFourGroup))
            {
                monster.DropItemGroups.Add(boxFourGroup);
            }

            monster.NumberOfMaximumItemDrops = 1;
        }

        return ValueTask.CompletedTask;
    }
}
