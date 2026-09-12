// <copyright file="RestrictInstantServerDropsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Restricts x9999 monster rewards and expands Potion Girl Amy's utility stock.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("4C7EAD89-1B15-443E-9BF1-1591D5269A22")]
public sealed class RestrictInstantServerDropsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Restrict x9999 Monster Drops";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Leaves monsters with Zen, low-rate jewels, and Kundun boxes, and adds class-change and wing-crafting items to Potion Girl Amy.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RestrictInstantServerDrops;

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

        InstantServerConfiguration.Apply(context, gameConfiguration);

        foreach (var item in detachedItems.Where(item => !IsItemReferenced(gameConfiguration, accounts, item)))
        {
            await context.DeleteAsync(item).ConfigureAwait(false);
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
}
