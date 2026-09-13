// <copyright file="InstantServerWingCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;

/// <summary>
/// Creates instant-server wings with guaranteed Luck and a maximum normal option.
/// </summary>
public class InstantServerWingCrafting : SimpleItemCraftingHandler
{
    private readonly SimpleCraftingSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="InstantServerWingCrafting"/> class.
    /// </summary>
    /// <param name="settings">The crafting settings.</param>
    public InstantServerWingCrafting(SimpleCraftingSettings settings)
        : base(settings)
    {
        this._settings = settings;
    }

    /// <inheritdoc/>
    protected override void AddRandomLuckOption(Item resultItem, Player player, byte successRate)
    {
        if (resultItem.Definition!.PossibleItemOptions
                .SelectMany(definition => definition.PossibleOptions)
                .FirstOrDefault(option => option.OptionType == ItemOptionTypes.Luck) is { } luck)
        {
            var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
            link.ItemOption = luck;
            resultItem.ItemOptions.Add(link);
        }
    }

    /// <inheritdoc/>
    protected override void AddRandomItemOption(Item resultItem, Player player, byte successRate)
    {
        var options = resultItem.Definition!.PossibleItemOptions
            .SelectMany(definition => definition.PossibleOptions)
            .Where(option => option.OptionType == ItemOptionTypes.Option)
            .ToList();
        var option = options.FirstOrDefault(option => option.PowerUpDefinition?.TargetAttribute == Stats.HealthRecoveryMultiplier)
                     ?? options.FirstOrDefault();
        if (option is not null)
        {
            var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
            link.ItemOption = option;
            link.Level = 4;
            resultItem.ItemOptions.Add(link);
        }
    }

    /// <inheritdoc/>
    protected override void AddRandomExcellentOptions(Item resultItem, Player player)
    {
        var options = resultItem.Definition!.PossibleItemOptions
            .SelectMany(definition => definition.PossibleOptions)
            .Where(option => option.OptionType == ItemOptionTypes.Wing);
        foreach (var option in options.Where(_ => Rand.NextRandomBool(this._settings.ResultItemExcellentOptionChance)))
        {
            var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
            link.ItemOption = option;
            resultItem.ItemOptions.Add(link);
        }
    }
}
