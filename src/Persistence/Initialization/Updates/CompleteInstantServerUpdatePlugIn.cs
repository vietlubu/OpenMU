// <copyright file="CompleteInstantServerUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Completes the x9999 instant-server configuration for existing Season 6 databases.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("3E9B7B8C-5F2A-4D0E-9A31-6C7D8E2F1B40")]
public sealed class CompleteInstantServerUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Complete x9999 Instant Server";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Adds curated excellent starter shops, tiered gacha boxes, repaired character level-up points, and rotating boss invasions.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.CompleteInstantServer;

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
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var accounts = (await context.GetAsync<Account>().ConfigureAwait(false)).ToList();
        var detachedItems = gameConfiguration.Monsters
            .Where(monster => InstantServerConfiguration.RebuiltMerchantNumbers.Contains(monster.Number))
            .SelectMany(monster => monster.MerchantStore?.Items ?? [])
            .ToList();
        var kundunBox = gameConfiguration.Items.First(item => item.Group == 14 && item.Number == 11);
        var detachedBoxGroups = kundunBox.DropItems
            .Where(group => group.SourceItemLevel is >= 8 and <= 12 && group.ItemType != SpecialItemType.Excellent)
            .ToList();
        var detachedLegacyGroups = gameConfiguration.DropItemGroups
            .Where(group => group.Monster is not null
                            && group.ItemLevel is >= 8 and <= 12
                            && group.PossibleItems.Count == 1
                            && group.PossibleItems.Single() == kundunBox)
            .ToList();

        InstantServerConfiguration.Apply(context, gameConfiguration);
        InstantServerConfiguration.ConfigureBossEvents(gameConfiguration);
        RepairCharacterLevelUpPoints(context, gameConfiguration, accounts.SelectMany(account => account.Characters));

        foreach (var server in await context.GetAsync<GameServerDefinition>().ConfigureAwait(false))
        {
            server.ExperienceRate = 1.0f;
            server.PvpEnabled = true;
        }

        foreach (var item in detachedItems.Where(item => !IsItemReferenced(gameConfiguration, accounts, item)))
        {
            await context.DeleteAsync(item).ConfigureAwait(false);
        }

        foreach (var group in detachedBoxGroups.Concat(detachedLegacyGroups).Distinct().Where(group => !IsDropGroupReferenced(gameConfiguration, accounts, group)))
        {
            await context.DeleteAsync(group).ConfigureAwait(false);
        }
    }

    private static void RepairCharacterLevelUpPoints(IContext context, GameConfiguration gameConfiguration, IEnumerable<Character> characters)
    {
        var pointsDefinition = Stats.PointsPerLevelUp.GetPersistent(gameConfiguration);
        var heroDefinition = Stats.GainHeroStatusQuestCompleted.GetPersistent(gameConfiguration);
        foreach (var character in characters)
        {
            var points = character.Attributes.FirstOrDefault(attribute => attribute.Definition == pointsDefinition);
            var heroBonus = character.Attributes.FirstOrDefault(attribute => attribute.Definition == heroDefinition)?.Value > 0 ? 1f : 0f;
            var repairedValue = InstantServerConfiguration.PointsPerLevel + heroBonus;
            if (points is null)
            {
                character.Attributes.Add(context.CreateNew<StatAttribute>(pointsDefinition, repairedValue));
            }
            else if (points.Value < InstantServerConfiguration.PointsPerLevel)
            {
                points.Value = repairedValue;
            }
        }
    }

    private static bool IsItemReferenced(GameConfiguration gameConfiguration, IEnumerable<Account> accounts, Item item)
    {
        if (gameConfiguration.Monsters.Any(monster => monster.MerchantStore?.Items.Contains(item) == true))
        {
            return true;
        }

        return accounts.Any(account => account.Vault?.Items.Contains(item) == true
                                       || account.Characters.Any(character => character.Inventory?.Items.Contains(item) == true));
    }

    private static bool IsDropGroupReferenced(GameConfiguration gameConfiguration, IEnumerable<Account> accounts, DropItemGroup group)
    {
        return gameConfiguration.DropItemGroups.Contains(group)
               || gameConfiguration.Items.Any(item => item.DropItems.Contains(group))
               || gameConfiguration.Monsters.Any(monster => monster.DropItemGroups.Contains(group))
               || gameConfiguration.Maps.Any(map => map.DropItemGroups.Contains(group))
               || accounts.SelectMany(account => account.Characters).Any(character => character.DropItemGroups.Contains(group));
    }
}
