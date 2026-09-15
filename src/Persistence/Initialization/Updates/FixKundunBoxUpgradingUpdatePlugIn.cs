// <copyright file="FixKundunBoxUpgradingUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Fixes the minimum and maximum item levels for the Kundun Box upgrading recipes.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("a3b4c5d6-e7f8-9a0b-1c2d-3e4f5a6b7c8d")]
public sealed class FixKundunBoxUpgradingUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Fix Kundun Box Upgrading Crafting Levels";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Fixes the item level requirements for Kundun Box recipes in the Chaos Machine.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixKundunBoxUpgrading;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 15, 12, 0, 0, DateTimeKind.Utc);

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

        var recipes = chaosGoblin.ItemCraftings.Where(c => c.Number >= 101 && c.Number <= 104).ToList();
        foreach (var crafting in recipes)
        {
            var sourceLevel = (byte)(crafting.Number - 101 + 8); // 101 -> 8, 102 -> 9, 103 -> 10, 104 -> 11

            var boxReq = crafting.SimpleCraftingSettings?.RequiredItems.FirstOrDefault(r => r.MinimumAmount == 10);
            if (boxReq != null)
            {
                boxReq.MinimumItemLevel = sourceLevel;
                boxReq.MaximumItemLevel = sourceLevel;
            }
        }

        return ValueTask.CompletedTask;
    }
}
