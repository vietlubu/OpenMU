// <copyright file="AddKundunBoxUpgradingUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds Chaos Machine recipes to upgrade 10 Box of Kundun to the next level.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("f2b5a1b3-4c5d-4f1b-8f3a-7f6b5c4d3e2a")]
public sealed class AddKundunBoxUpgradingUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Add Kundun Box Upgrading Crafting";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Adds Chaos Machine recipes to upgrade 10 Box of Kundun to the next level.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddKundunBoxUpgrading;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 15, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var chaosGoblin = gameConfiguration.Monsters.FirstOrDefault(m => m.NpcWindow == NpcWindow.ChaosMachine);
        if (chaosGoblin == null)
        {
            return ValueTask.CompletedTask;
        }

        chaosGoblin.ItemCraftings.Add(this.CreateKundunBoxCrafting(context, gameConfiguration, 8, 100, 101));
        chaosGoblin.ItemCraftings.Add(this.CreateKundunBoxCrafting(context, gameConfiguration, 9, 90, 102));
        chaosGoblin.ItemCraftings.Add(this.CreateKundunBoxCrafting(context, gameConfiguration, 10, 80, 103));
        chaosGoblin.ItemCraftings.Add(this.CreateKundunBoxCrafting(context, gameConfiguration, 11, 70, 104));

        return ValueTask.CompletedTask;
    }

    private ItemCrafting CreateKundunBoxCrafting(IContext context, GameConfiguration gameConfiguration, byte sourceLevel, byte successRate, byte mixNumber)
    {
        var crafting = context.CreateNew<ItemCrafting>();
        crafting.Name = $"Box of Kundun +{(sourceLevel - 7)} to +{(sourceLevel - 6)}";
        crafting.Number = mixNumber;

        var craftingSettings = context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.Money = 0;
        craftingSettings.SuccessPercent = successRate;
        craftingSettings.MultipleAllowed = false;

        // 10 Box of Kundun of sourceLevel
        var boxReq = context.CreateNew<ItemCraftingRequiredItem>();
        boxReq.MinimumAmount = 10;
        boxReq.MaximumAmount = 10;
        boxReq.MinimumItemLevel = sourceLevel;
        boxReq.MaximumItemLevel = sourceLevel;
        boxReq.SuccessResult = MixResult.Disappear;
        boxReq.FailResult = MixResult.Disappear;
        boxReq.PossibleItems.Add(gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 11)); // Box of Luck / Kundun
        craftingSettings.RequiredItems.Add(boxReq);

        // 1 Jewel of Chaos
        var chaosReq = context.CreateNew<ItemCraftingRequiredItem>();
        chaosReq.MinimumAmount = 1;
        chaosReq.MaximumAmount = 1;
        chaosReq.SuccessResult = MixResult.Disappear;
        chaosReq.FailResult = MixResult.Disappear;
        chaosReq.PossibleItems.Add(gameConfiguration.Items.First(i => i.Name == "Jewel of Chaos"));
        craftingSettings.RequiredItems.Add(chaosReq);

        // Result: Box of Kundun of sourceLevel + 1
        var result = context.CreateNew<ItemCraftingResultItem>();
        result.ItemDefinition = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 11);
        result.RandomMinimumLevel = (byte)(sourceLevel + 1);
        result.RandomMaximumLevel = (byte)(sourceLevel + 1);
        craftingSettings.ResultItems.Add(result);

        return crafting;
    }
}
