// <copyright file="InstantServerWingCraftingTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// Tests the guaranteed instant-server wing options.
/// </summary>
[TestFixture]
public class InstantServerWingCraftingTests
{
    /// <summary>
    /// Wings with a recovery option always receive it at the maximum normal-option level.
    /// </summary>
    [Test]
    public async Task PrefersMaximumHealthRecoveryOptionAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var recovery = CreateOption(player, ItemOptionTypes.Option, Stats.HealthRecoveryMultiplier, 0);
        var damage = CreateOption(player, ItemOptionTypes.Option, Stats.PhysicalBaseDmg, 1);
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = CreateDefinition(player, recovery, damage);
        var handler = new TestableInstantServerWingCrafting(new SimpleCraftingSettings());

        handler.AddOptions(item, player);

        var option = item.ItemOptions.Single(link => link.ItemOption?.OptionType == ItemOptionTypes.Option);
        Assert.Multiple(() =>
        {
            Assert.That(option.ItemOption, Is.SameAs(recovery));
            Assert.That(option.Level, Is.EqualTo(4));
        });
    }

    /// <summary>
    /// Wings without a recovery record retain their first native normal option.
    /// </summary>
    [Test]
    public async Task FallsBackToNativeNormalOptionAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var damage = CreateOption(player, ItemOptionTypes.Option, Stats.PhysicalBaseDmg, 1);
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = CreateDefinition(player, damage);
        var handler = new TestableInstantServerWingCrafting(new SimpleCraftingSettings());

        handler.AddOptions(item, player);

        Assert.That(item.ItemOptions.Single().ItemOption, Is.SameAs(damage));
    }

    /// <summary>
    /// Luck is always applied and a 100% special-option rate produces every supported line.
    /// </summary>
    [Test]
    public async Task AddsLuckAndAllSpecialOptionsAtFullRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var luck = CreateOption(player, ItemOptionTypes.Luck, Stats.MaximumHealth, 0);
        var specialOptions = Enumerable.Range(1, 4)
            .Select(number => CreateOption(player, ItemOptionTypes.Wing, Stats.MaximumHealth, number))
            .ToArray();
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = CreateDefinition(player, [luck, .. specialOptions]);
        var handler = new TestableInstantServerWingCrafting(new SimpleCraftingSettings { ResultItemExcellentOptionChance = 100 });

        handler.AddOptions(item, player);

        Assert.Multiple(() =>
        {
            Assert.That(item.ItemOptions.Count(link => link.ItemOption?.OptionType == ItemOptionTypes.Luck), Is.EqualTo(1));
            Assert.That(item.ItemOptions.Where(link => link.ItemOption?.OptionType == ItemOptionTypes.Wing).Select(link => link.ItemOption), Is.EquivalentTo(specialOptions));
        });
    }

    private static ItemDefinition CreateDefinition(Player player, params IncreasableItemOption[] options)
    {
        var definition = player.PersistenceContext.CreateNew<ItemDefinition>();
        foreach (var group in options.GroupBy(option => option.OptionType))
        {
            var optionDefinition = player.PersistenceContext.CreateNew<ItemOptionDefinition>();
            foreach (var option in group)
            {
                optionDefinition.PossibleOptions.Add(option);
            }

            definition.PossibleItemOptions.Add(optionDefinition);
        }

        return definition;
    }

    private static IncreasableItemOption CreateOption(Player player, ItemOptionType type, AttributeDefinition target, int number)
    {
        var option = player.PersistenceContext.CreateNew<IncreasableItemOption>();
        option.Number = number;
        option.OptionType = type;
        option.PowerUpDefinition = player.PersistenceContext.CreateNew<PowerUpDefinition>();
        option.PowerUpDefinition.TargetAttribute = target;
        return option;
    }

    private sealed class TestableInstantServerWingCrafting(SimpleCraftingSettings settings) : InstantServerWingCrafting(settings)
    {
        internal void AddOptions(Item item, Player player)
        {
            this.AddRandomLuckOption(item, player, 90);
            this.AddRandomItemOption(item, player, 90);
            this.AddRandomExcellentOptions(item, player);
        }
    }
}
