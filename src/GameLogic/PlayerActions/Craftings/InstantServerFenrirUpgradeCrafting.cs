// <copyright file="InstantServerFenrirUpgradeCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// Fenrir upgrade crafting with the instant-server success rate.
/// </summary>
public class InstantServerFenrirUpgradeCrafting : FenrirUpgradeCrafting
{
    /// <inheritdoc />
    public override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems)
    {
        var result = base.TryGetRequiredItems(player, out items, out successRateByItems);
        if (result is null)
        {
            successRateByItems = 100;
        }

        return result;
    }
}
