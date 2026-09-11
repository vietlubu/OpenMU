// <copyright file="InstantServerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Applies the high-rate, instant-server settings used by the Season 6 realm.
/// </summary>
internal static class InstantServerConfiguration
{
    /// <summary>
    /// Gets the global experience multiplier.
    /// </summary>
    internal const float ExperienceRate = 9999f;

    /// <summary>
    /// Gets the stat points granted by each level-up.
    /// </summary>
    internal const float PointsPerLevel = 500f;

    /// <summary>
    /// Gets the minimum monsters in every permanent hunting pack.
    /// </summary>
    internal const short MonsterPackSize = 10;

    /// <summary>
    /// Gets the multiplier applied when picked-up Zen is credited.
    /// </summary>
    internal const float MoneyAmountRate = 1_000f;

    /// <summary>
    /// Gets the respawn delay of regular monsters.
    /// </summary>
    internal static readonly TimeSpan MonsterRespawnDelay = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Applies settings which are stored on the game configuration.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void Apply(IContext context, GameConfiguration gameConfiguration)
    {
        gameConfiguration.ExperienceRate = ExperienceRate;
        gameConfiguration.AreaSkillHitsPlayer = true;
        gameConfiguration.ExcellentItemDropLevelDelta = 0;
        ConfigureMoneyAmountRate(context, gameConfiguration);
        ConfigureDropRates(gameConfiguration);
        ConfigureLevelUpPoints(gameConfiguration);
        ConfigureMonsterPacks(gameConfiguration);
        ConfigurePvp(gameConfiguration);
        ConfigureMerchantStores(context, gameConfiguration);
    }

    /// <summary>
    /// Activates and staggers the built-in boss invasions so at least one is normally running.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureBossEvents(GameConfiguration gameConfiguration)
    {
        ConfigureBossEvent<GoldenInvasionPlugIn>(gameConfiguration, TimeSpan.Zero);
        ConfigureBossEvent<RedDragonInvasionPlugIn>(gameConfiguration, TimeSpan.FromMinutes(3));
        ConfigureBossEvent<WhiteWizardInvasionPlugIn>(gameConfiguration, TimeSpan.FromMinutes(6));
    }

    /// <summary>
    /// Raises the amount of Zen credited from every money drop.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureMoneyAmountRate(IContext context, GameConfiguration gameConfiguration)
    {
        var moneyRate = gameConfiguration.GlobalBaseAttributeValues
            .FirstOrDefault(attribute => attribute.Definition?.Id == Stats.MoneyAmountRate.Id);
        if (moneyRate?.Value == MoneyAmountRate)
        {
            return;
        }

        if (moneyRate is not null)
        {
            gameConfiguration.GlobalBaseAttributeValues.Remove(moneyRate);
        }

        var definition = gameConfiguration.Attributes.First(attribute => attribute.Id == Stats.MoneyAmountRate.Id);
        gameConfiguration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(MoneyAmountRate, definition, AggregateType.AddRaw));
    }

    private static void ConfigureBossEvent<TPlugIn>(GameConfiguration gameConfiguration, TimeSpan offset)
        where TPlugIn : SimpleInvasionPlugIn, new()
    {
        var plugInConfiguration = gameConfiguration.PlugInConfigurations.FirstOrDefault(configuration => configuration.TypeId == typeof(TPlugIn).GUID);
        if (plugInConfiguration is null)
        {
            return;
        }

        var configuration = (PeriodicInvasionConfiguration)new TPlugIn().CreateDefaultConfig();
        configuration.PreStartMessageDelay = TimeSpan.Zero;
        configuration.TaskDuration = TimeSpan.FromMinutes(8);
        configuration.Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(
                TimeSpan.FromMinutes(10),
                TimeOnly.FromTimeSpan(offset))
            .ToList();
        foreach (var mob in configuration.Mobs)
        {
            mob.Count = Math.Min((ushort)254, (ushort)(mob.Count * 2));
        }

        plugInConfiguration.IsActive = true;
        plugInConfiguration.SetConfiguration(configuration, null);
    }

    private static void ConfigureDropRates(GameConfiguration gameConfiguration)
    {
        var defaultDropGroups = new HashSet<Guid>
        {
            GuidHelper.CreateGuid<DropItemGroup>(1),
            GuidHelper.CreateGuid<DropItemGroup>(2),
            GuidHelper.CreateGuid<DropItemGroup>(3),
            GuidHelper.CreateGuid<DropItemGroup>(4),
        };

        foreach (var dropGroup in gameConfiguration.DropItemGroups.Where(group => defaultDropGroups.Contains(group.GetId())))
        {
            dropGroup.Chance = 1.0;
        }

        foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.ObjectKind == NpcObjectKind.Monster))
        {
            monster.NumberOfMaximumItemDrops = Math.Max((byte)4, monster.NumberOfMaximumItemDrops);
            monster.RespawnDelay = MonsterRespawnDelay;
        }
    }

    private static void ConfigureLevelUpPoints(GameConfiguration gameConfiguration)
    {
        foreach (var characterClass in gameConfiguration.CharacterClasses)
        {
            if (characterClass.StatAttributes.FirstOrDefault(attribute => attribute.Attribute == Stats.PointsPerLevelUp) is { } pointsPerLevel)
            {
                pointsPerLevel.BaseValue = PointsPerLevel;
            }
        }

        foreach (var stat in new[] { Stats.BaseStrength, Stats.BaseAgility, Stats.BaseVitality, Stats.BaseEnergy, Stats.BaseLeadership })
        {
            if (gameConfiguration.Attributes.FirstOrDefault(attribute => attribute == stat) is { } persistentStat)
            {
                persistentStat.MaximumValue = 32_767;
            }
        }
    }

    private static void ConfigureMonsterPacks(GameConfiguration gameConfiguration)
    {
        var permanentMonsterSpawns = gameConfiguration.Maps
            .SelectMany(map => map.MonsterSpawns)
            .Where(spawn => spawn is { SpawnTrigger: SpawnTrigger.Automatic, MonsterDefinition.ObjectKind: NpcObjectKind.Monster });

        foreach (var spawn in permanentMonsterSpawns)
        {
            spawn.Quantity = Math.Max(MonsterPackSize, spawn.Quantity);
            if (spawn.X1 == spawn.X2 && spawn.Y1 == spawn.Y2)
            {
                spawn.X1 = (byte)Math.Max(0, spawn.X1 - 2);
                spawn.X2 = (byte)Math.Min(byte.MaxValue, spawn.X2 + 2);
                spawn.Y1 = (byte)Math.Max(0, spawn.Y1 - 2);
                spawn.Y2 = (byte)Math.Min(byte.MaxValue, spawn.Y2 + 2);
            }
        }
    }

    private static void ConfigurePvp(GameConfiguration gameConfiguration)
    {
        foreach (var miniGame in gameConfiguration.MiniGameDefinitions)
        {
            miniGame.ArePlayerKillersAllowedToEnter = true;
        }
    }

    private static void ConfigureMerchantStores(IContext context, GameConfiguration gameConfiguration)
    {
        var stores = gameConfiguration.Monsters
            .Where(monster => monster.MerchantStore is not null)
            .OrderBy(monster => monster.Number)
            .Select(monster => monster.MerchantStore!)
            .Distinct<ItemStorage>(ReferenceEqualityComparer.Instance)
            .ToList();

        if (stores.Count == 0)
        {
            return;
        }

        var packers = stores.Select(store => new MerchantStorePacker(store)).ToList();
        var soldDefinitions = stores
            .SelectMany(store => store.Items)
            .Where(item => item.Definition is not null)
            .Select(item => item.Definition!)
            .ToHashSet();
        var shopItems = gameConfiguration.Items
            .Where(item => IsShopItem(item) && !soldDefinitions.Contains(item))
            .OrderBy(GetShopPriority)
            .ThenBy(item => item.Group)
            .ThenBy(item => item.Number);

        foreach (var definition in shopItems)
        {
            MerchantStorePacker? packer = null;
            byte slot = 0;
            foreach (var candidate in packers)
            {
                if (candidate.TryReserve(definition, out slot))
                {
                    packer = candidate;
                    break;
                }
            }

            if (packer is null)
            {
                continue;
            }

            var item = context.CreateNew<Item>();
            item.Definition = definition;
            item.ItemSlot = slot;
            item.Durability = Math.Max(1d, definition.Durability);
            item.Level = 0;
            item.HasSkill = definition.ItemSlot is not null && definition.Skill is not null;
            item.SocketCount = 0;
            packer.Store.Items.Add(item);
        }
    }

    private static bool IsShopItem(ItemDefinition item)
        => !item.IsQuestItem
           && item.Group <= 15
           && item.Number is >= 0 and <= byte.MaxValue
           && item.Width > 0
           && item.Width <= InventoryConstants.RowSize
           && item.Height > 0
           && item.Height <= InventoryConstants.WarehouseRows;

    private static int GetShopPriority(ItemDefinition item)
        => item.Group >= 12 || item.Skill is not null ? 0 : 1;

    private sealed class MerchantStorePacker
    {
        private readonly bool[,] _occupied = new bool[InventoryConstants.WarehouseRows, InventoryConstants.RowSize];

        internal MerchantStorePacker(ItemStorage store)
        {
            _ = new Storage(InventoryConstants.WarehouseSize, store);
            this.Store = store;
            foreach (var item in store.Items.Where(item => item.Definition is not null))
            {
                this.MarkExisting(item);
            }
        }

        internal ItemStorage Store { get; }

        internal bool TryReserve(ItemDefinition definition, out byte slot)
        {
            for (var row = 0; row <= InventoryConstants.WarehouseRows - definition.Height; row++)
            {
                for (var column = 0; column <= InventoryConstants.RowSize - definition.Width; column++)
                {
                    if (!this.Fits(row, column, definition.Width, definition.Height))
                    {
                        continue;
                    }

                    slot = (byte)((row * InventoryConstants.RowSize) + column);
                    this.MarkOccupied(row, column, definition.Width, definition.Height);

                    return true;
                }
            }

            slot = 0;
            return false;
        }

        private void MarkExisting(Item item)
        {
            var definition = item.Definition!;
            var row = item.ItemSlot / InventoryConstants.RowSize;
            var column = item.ItemSlot % InventoryConstants.RowSize;
            if (row + definition.Height <= InventoryConstants.WarehouseRows
                && column + definition.Width <= InventoryConstants.RowSize)
            {
                this.MarkOccupied(row, column, definition.Width, definition.Height);
            }
        }

        private bool Fits(int row, int column, int width, int height)
        {
            for (var y = row; y < row + height; y++)
            {
                for (var x = column; x < column + width; x++)
                {
                    if (this._occupied[y, x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void MarkOccupied(int row, int column, int width, int height)
        {
            for (var y = row; y < row + height; y++)
            {
                for (var x = column; x < column + width; x++)
                {
                    this._occupied[y, x] = true;
                }
            }
        }
    }
}
