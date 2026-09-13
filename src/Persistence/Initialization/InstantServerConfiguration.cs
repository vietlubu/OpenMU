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
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Persistence.Initialization.Items;
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
    /// Gets the points granted per level.
    /// </summary>
    internal const float PointsPerLevel = 500f;

    /// <summary>
    /// Gets the minimum automatic spawn quantity.
    /// </summary>
    internal const short MonsterPackSize = 10;

    /// <summary>
    /// Gets the picked-up Zen multiplier.
    /// </summary>
    internal const float MoneyAmountRate = 1_000f;

    /// <summary>
    /// Gets the regular-monster jewel drop chance.
    /// </summary>
    internal const double JewelDropChance = 0.01;

    /// <summary>
    /// Gets the success rate of every wing and cape crafting.
    /// </summary>
    internal const byte WingCraftingSuccessRate = 90;

    /// <summary>
    /// Gets the independent chance for each supported special wing option.
    /// </summary>
    internal const byte WingSpecialOptionChance = 90;

    /// <summary>
    /// Gets the regular monster respawn delay.
    /// </summary>
    internal static readonly TimeSpan MonsterRespawnDelay = TimeSpan.FromSeconds(5);

    private static readonly byte[] DarkWizardClasses = [0, 2, 3];
    private static readonly byte[] DarkKnightClasses = [4, 6, 7];
    private static readonly byte[] FairyElfClasses = [8, 10, 11];
    private static readonly byte[] MagicGladiatorClasses = [12, 13];
    private static readonly byte[] DarkLordClasses = [16, 17];
    private static readonly byte[] SummonerClasses = [20, 22, 23];
    private static readonly byte[] RageFighterClasses = [24, 25];
    private static readonly short[] SpecialistMerchantNumbers = [230, 242, 243, 245, 246, 251, 254, 416, 417];
    private static readonly short[] GeneralGoodsMerchantNumbers = [253, 259, 376, 377, 415, 545, 577];
    private static readonly HashSet<short> BossMonsterNumbers =
    [
        43, 44, 53, 54, 78, 79, 80, 81, 82, 83, 135, 161, 181, 189, 197, 267, 275, 295, 338, 361, 362, 363, 364, 440, 459,
    ];

    /// <summary>
    /// Gets the merchants whose stores are rebuilt by this configuration.
    /// </summary>
    internal static IReadOnlyCollection<short> RebuiltMerchantNumbers { get; } = SpecialistMerchantNumbers.Concat(GeneralGoodsMerchantNumbers).ToArray();

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
        ConfigurePotionStacks(gameConfiguration);
        ConfigureMerchantStores(context, gameConfiguration);
        ConfigureGacha(context, gameConfiguration);
        ConfigureGuaranteedLuck(context, gameConfiguration);
        ConfigureWingCraftings(gameConfiguration);
    }

    /// <summary>
    /// Configures the built-in boss invasions as a non-overlapping thirty-minute circuit.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureBossEvents(GameConfiguration gameConfiguration)
    {
        ConfigureBossEvent<GoldenInvasionPlugIn>(gameConfiguration, TimeSpan.Zero);
        ConfigureBossEvent<RedDragonInvasionPlugIn>(gameConfiguration, TimeSpan.FromMinutes(10));
        ConfigureBossEvent<WhiteWizardInvasionPlugIn>(gameConfiguration, TimeSpan.FromMinutes(20));
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

    /// <summary>
    /// Repairs the points-per-level class templates.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureLevelUpPoints(GameConfiguration gameConfiguration)
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

    /// <summary>
    /// Guarantees Luck on instant-server shop equipment and Box of Kundun rewards.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureGuaranteedLuck(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var item in gameConfiguration.Monsters
                     .Where(monster => RebuiltMerchantNumbers.Contains(monster.Number))
                     .SelectMany(monster => monster.MerchantStore?.Items ?? []))
        {
            var luck = item.Definition?.PossibleItemOptions
                .SelectMany(definition => definition.PossibleOptions)
                .FirstOrDefault(option => option.OptionType == ItemOptionTypes.Luck);
            if (luck is not null && item.ItemOptions.All(link => link.ItemOption != luck))
            {
                var link = context.CreateNew<ItemOptionLink>();
                link.ItemOption = luck;
                item.ItemOptions.Add(link);
            }
        }

        var kundunBox = GetItemDefinition(gameConfiguration, 14, 11);
        foreach (var group in kundunBox.DropItems.Where(group => group.SourceItemLevel is >= 8 and <= 12))
        {
            group.ItemType = SpecialItemType.ExcellentWithLuck;
        }
    }

    /// <summary>
    /// Configures all wing and cape mixes with fixed instant-server result rates and options.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigureWingCraftings(GameConfiguration gameConfiguration)
    {
        var craftings = gameConfiguration.Monsters.Single(monster => monster.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings;
        foreach (var crafting in craftings.Where(crafting => crafting.Number is 7 or 11 or 24 or 38 or 39))
        {
            crafting.ItemCraftingHandlerClassName = typeof(InstantServerWingCrafting).FullName!;
            if (crafting.SimpleCraftingSettings is not { } settings)
            {
                continue;
            }

            settings.SuccessPercent = WingCraftingSuccessRate;
            settings.MaximumSuccessPercent = WingCraftingSuccessRate;
            settings.NpcPriceDivisor = 0;
            settings.SuccessPercentageAdditionForLuck = 0;
            settings.SuccessPercentageAdditionForExcellentItem = 0;
            settings.SuccessPercentageAdditionForAncientItem = 0;
            settings.SuccessPercentageAdditionForGuardianItem = 0;
            settings.SuccessPercentageAdditionForSocketItem = 0;
            settings.ResultItemLuckOptionChance = 100;
            settings.ResultItemExcellentOptionChance = WingSpecialOptionChance;
            settings.ResultItemMaxExcOptionCount = 4;
            foreach (var requiredItem in settings.RequiredItems)
            {
                requiredItem.AddPercentage = 0;
                requiredItem.NpcPriceDivisor = 0;
            }
        }
    }

    /// <summary>
    /// Rebuilds Potion Girl Amy's store with the instant-server utility and crafting stock.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void ConfigurePotionGirlStore(IContext context, GameConfiguration gameConfiguration)
    {
        ConfigureSpecialistStore(context, gameConfiguration, 253, packer =>
        {
            AddGeneralGoods(context, gameConfiguration, packer);
            AddClassChangeAndWingItems(context, gameConfiguration, packer);
        });
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
        configuration.TaskDuration = TimeSpan.FromMinutes(10);
        configuration.Timetable = PeriodicTaskConfiguration.GenerateTimeSequence(
                TimeSpan.FromMinutes(30),
                TimeOnly.FromTimeSpan(offset))
            .ToList();
        plugInConfiguration.IsActive = true;
        plugInConfiguration.SetConfiguration(configuration, null);
    }

    private static void ConfigureDropRates(GameConfiguration gameConfiguration)
    {
        var moneyGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(1));
        var jewelGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(4));
        moneyGroup.Chance = 1.0;
        jewelGroup.Chance = JewelDropChance;

        foreach (var map in gameConfiguration.Maps)
        {
            map.DropItemGroups.Clear();
            map.DropItemGroups.Add(moneyGroup);
        }

        foreach (var requiredItem in gameConfiguration.Monsters
                     .SelectMany(monster => monster.Quests)
                     .SelectMany(quest => quest.RequiredItems)
                     .Where(requiredItem => requiredItem.Item?.IsQuestItem == true))
        {
            requiredItem.DropItemGroup = null;
        }

        foreach (var monster in gameConfiguration.Monsters.Where(monster => monster.ObjectKind == NpcObjectKind.Monster))
        {
            monster.DropItemGroups.Clear();
            monster.NumberOfMaximumItemDrops = 2;
            monster.RespawnDelay = MonsterRespawnDelay;
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

    private static void ConfigurePotionStacks(GameConfiguration gameConfiguration)
    {
        GetItemDefinition(gameConfiguration, 14, 3).Durability = byte.MaxValue;
        GetItemDefinition(gameConfiguration, 14, 6).Durability = byte.MaxValue;
    }

    private static void ConfigureMerchantStores(IContext context, GameConfiguration gameConfiguration)
    {
        ConfigureSpecialistStore(context, gameConfiguration, 254, packer =>
        {
            AddEquipmentProfile(context, gameConfiguration, packer, DarkWizardClasses, 2, [(5, 0), (5, 2)]);
            AddSkillItems(context, gameConfiguration, packer, DarkWizardClasses.Concat(MagicGladiatorClasses));
        });
        ConfigureSpecialistStore(context, gameConfiguration, 251, packer =>
        {
            AddEquipmentProfile(context, gameConfiguration, packer, DarkKnightClasses, 5, [(0, 5), (0, 6)]);
            AddEquipmentProfile(context, gameConfiguration, packer, MagicGladiatorClasses, 15, [(0, 5), (5, 0)]);
            AddEquipmentProfile(context, gameConfiguration, packer, DarkLordClasses, 25, [(2, 8), (2, 9)]);
            AddEquipmentProfile(context, gameConfiguration, packer, RageFighterClasses, 59, [(0, 32), (0, 33)]);
        });
        ConfigureSpecialistStore(context, gameConfiguration, 230, packer =>
            AddSkillItems(context, gameConfiguration, packer, DarkKnightClasses.Concat(DarkLordClasses).Concat(RageFighterClasses)));
        ConfigureSpecialistStore(context, gameConfiguration, 242, packer => AddSkillItems(context, gameConfiguration, packer, FairyElfClasses));
        ConfigureSpecialistStore(context, gameConfiguration, 243, packer =>
            AddEquipmentProfile(context, gameConfiguration, packer, FairyElfClasses, 10, [(4, 0), (4, 3)]));
        ConfigureSpecialistStore(context, gameConfiguration, 416, packer =>
            AddEquipmentProfile(context, gameConfiguration, packer, SummonerClasses, 40, [(5, 15), (5, 21), (5, 22)]));
        ConfigureSpecialistStore(context, gameConfiguration, 417, packer => AddSkillItems(context, gameConfiguration, packer, SummonerClasses));
        ConfigureSpecialistStore(context, gameConfiguration, 245, packer =>
        {
            AddEquipmentProfile(context, gameConfiguration, packer, DarkWizardClasses, 2, [(5, 0), (5, 2)]);
            AddSkillItems(context, gameConfiguration, packer, DarkWizardClasses.Concat(MagicGladiatorClasses));
        });
        ConfigureSpecialistStore(context, gameConfiguration, 246, packer =>
        {
            // ponytail: the client exposes only 120 merchant slots; complete armor sets remain in their reachable home-town stores.
            AddWeaponProfile(
                context,
                gameConfiguration,
                packer,
                DarkKnightClasses.Concat(FairyElfClasses).Concat(MagicGladiatorClasses).Concat(DarkLordClasses).Concat(RageFighterClasses),
                [(0, 5), (0, 6), (4, 0), (4, 3), (5, 0), (2, 8), (2, 9), (0, 32), (0, 33)]);
        });

        foreach (var npcNumber in GeneralGoodsMerchantNumbers.Where(number => number != 253))
        {
            ConfigureSpecialistStore(context, gameConfiguration, npcNumber, packer => AddGeneralGoods(context, gameConfiguration, packer));
        }

        ConfigurePotionGirlStore(context, gameConfiguration);
    }

    private static void ConfigureSpecialistStore(IContext context, GameConfiguration gameConfiguration, short npcNumber, Action<MerchantStorePacker> populate)
    {
        var monster = gameConfiguration.Monsters.First(monster => monster.Number == npcNumber && monster.MerchantStore is not null);
        var store = monster.MerchantStore!;
        store.Items.Clear();
        var packer = new MerchantStorePacker(store, monster);
        populate(packer);
        packer.Complete();
    }

    private static void AddEquipmentProfile(
        IContext context,
        GameConfiguration gameConfiguration,
        MerchantStorePacker packer,
        IEnumerable<byte> classNumbers,
        byte armorSetNumber,
        IEnumerable<(byte Group, byte Number)> weapons)
    {
        var classes = classNumbers.ToHashSet();
        var itemHelper = new ItemHelper(context, gameConfiguration);
        foreach (var group in new[] { ItemGroups.Helm, ItemGroups.Armor, ItemGroups.Pants, ItemGroups.Gloves, ItemGroups.Boots })
        {
            var definition = gameConfiguration.Items.FirstOrDefault(item => item.Group == (byte)group
                                                                            && item.Number == armorSetNumber
                                                                            && item.QualifiedCharacters.Any(characterClass => classes.Contains(characterClass.Number)));
            if (definition is null)
            {
                continue;
            }

            var item = itemHelper.CreateSetItem(0, armorSetNumber, group, Stats.MaximumHealth, level: 7);
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            packer.Add(item);
        }

        foreach (var (group, number) in weapons)
        {
            var definition = GetItemDefinition(gameConfiguration, group, number);
            if (!definition.QualifiedCharacters.Any(characterClass => classes.Contains(characterClass.Number)))
            {
                continue;
            }

            var item = itemHelper.CreateWeapon(0, (ItemGroups)group, number, 7, 0, false, definition.Skill is not null, Stats.ExcellentDamageChance);
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            packer.Add(item);
        }
    }

    private static void AddWeaponProfile(
        IContext context,
        GameConfiguration gameConfiguration,
        MerchantStorePacker packer,
        IEnumerable<byte> classNumbers,
        IEnumerable<(byte Group, byte Number)> weapons)
    {
        var classes = classNumbers.ToHashSet();
        var itemHelper = new ItemHelper(context, gameConfiguration);
        foreach (var (group, number) in weapons.Distinct())
        {
            var definition = GetItemDefinition(gameConfiguration, group, number);
            if (!definition.QualifiedCharacters.Any(characterClass => classes.Contains(characterClass.Number)))
            {
                continue;
            }

            var item = itemHelper.CreateWeapon(0, (ItemGroups)group, number, 7, 0, false, definition.Skill is not null, Stats.ExcellentDamageChance);
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            packer.Add(item);
        }
    }

    private static void AddSkillItems(IContext context, GameConfiguration gameConfiguration, MerchantStorePacker packer, IEnumerable<byte> classNumbers)
    {
        var classes = classNumbers.ToHashSet();
        var definitions = gameConfiguration.Items
            .Where(item => item.Group is 12 or 15
                           && item.ItemSlot is null
                           && item.Skill is not null
                           && item.QualifiedCharacters.Any(characterClass => classes.Contains(characterClass.Number)))
            .OrderBy(item => item.Group)
            .ThenBy(item => item.Number)
            .ToList();

        foreach (var definition in definitions)
        {
            if (definition is { Group: 12, Number: 11 })
            {
                for (byte level = 0; level <= 6; level++)
                {
                    packer.Add(CreateStoreItem(context, definition, level: level));
                }
            }
            else
            {
                packer.Add(CreateStoreItem(context, definition));
            }
        }
    }

    private static void AddGeneralGoods(IContext context, GameConfiguration gameConfiguration, MerchantStorePacker packer)
    {
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 14, 3), byte.MaxValue, 1));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 14, 6), byte.MaxValue, 1));
        var antidote = GetItemDefinition(gameConfiguration, 14, 8);
        packer.Add(CreateStoreItem(context, antidote, Math.Max(1, (int)antidote.Durability)));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 4, 7)));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 4, 15)));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 14, 10)));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 13, 29)));
    }

    private static void AddClassChangeAndWingItems(IContext context, GameConfiguration gameConfiguration, MerchantStorePacker packer)
    {
        foreach (var (number, level) in new (byte Number, byte Level)[]
                 {
                     (23, 0), (23, 1), (24, 0), (24, 1), (25, 0), (26, 0), (65, 0), (66, 0), (67, 0), (68, 0),
                 })
        {
            packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 14, number), level: level));
        }

        var itemHelper = new ItemHelper(context, gameConfiguration);
        foreach (var (group, number) in new (ItemGroups Group, byte Number)[]
                 {
                     (ItemGroups.Scepters, 6), (ItemGroups.Bows, 6), (ItemGroups.Staff, 7),
                 })
        {
            var definition = GetItemDefinition(gameConfiguration, (byte)group, number);
            var item = itemHelper.CreateWeapon(0, group, number, 4, 1, true, definition.Skill is not null, null);
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            packer.Add(item);
        }

        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 13, 14)));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 13, 14), level: 1));
        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 13, 52)));

        foreach (var (group, number) in new (byte Group, short Number)[]
                 {
                     (14, 13), // Jewel of Bless
                     (14, 14), // Jewel of Soul
                     (12, 15), // Jewel of Chaos
                     (14, 16), // Jewel of Life
                     (14, 22), // Jewel of Creation
                 })
        {
            packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, group, number)));
        }

        foreach (var number in new short[] { 30, 31, 136, 137, 141 })
        {
            var definition = GetItemDefinition(gameConfiguration, 12, number);
            for (byte level = 0; level <= definition.MaximumItemLevel; level++)
            {
                packer.Add(CreateStoreItem(context, definition, level: level));
            }
        }

        packer.Add(CreateStoreItem(context, GetItemDefinition(gameConfiguration, 13, 53)));
    }

    private static Item CreateStoreItem(IContext context, ItemDefinition definition, double durability = 1, byte level = 0)
    {
        var item = context.CreateNew<Item>();
        item.Definition = definition;
        item.Durability = durability;
        item.Level = level;
        return item;
    }

    private static void ConfigureGacha(IContext context, GameConfiguration gameConfiguration)
    {
        var kundunBox = GetItemDefinition(gameConfiguration, 14, 11);
        foreach (var level in Enumerable.Range(8, 5).Select(level => (byte)level))
        {
            var excellentGroup = kundunBox.DropItems.Single(group => group.SourceItemLevel == level && group.ItemType is SpecialItemType.Excellent or SpecialItemType.ExcellentWithLuck);
            excellentGroup.Chance = 1.0;
            foreach (var obsolete in kundunBox.DropItems.Where(group => group.SourceItemLevel == level && group != excellentGroup).ToList())
            {
                kundunBox.DropItems.Remove(obsolete);
            }
        }

        ConfigureJackpotOpening(context, gameConfiguration, kundunBox);
        RemoveLegacyKundunMonsterDrops(gameConfiguration, kundunBox);

        var kundunOne = UpsertGachaGroup(context, gameConfiguration, 1, "Kundun +1 Gacha", 0.02, kundunBox, 8);
        var kundunTwo = UpsertGachaGroup(context, gameConfiguration, 2, "Kundun +2 Gacha", 0.015, kundunBox, 9);
        var kundunThree = UpsertGachaGroup(context, gameConfiguration, 3, "Kundun +3 Gacha", 0.01, kundunBox, 10);
        var kundunFour = UpsertGachaGroup(context, gameConfiguration, 4, "Kundun +4 Boss Gacha", 0.475, kundunBox, 11);
        var kundunFive = UpsertGachaGroup(context, gameConfiguration, 5, "Kundun +5 Boss Gacha", 0.475, kundunBox, 12);
        var jackpotBox = GetItemDefinition(gameConfiguration, 14, 52);
        var jackpot = UpsertGachaGroup(context, gameConfiguration, 6, "Full Option GM Gift Boss Gacha", 0.05, jackpotBox, 0);
        var jewelGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == GuidHelper.CreateGuid<DropItemGroup>(4));

        var configuredGroups = new[] { kundunOne, kundunTwo, kundunThree, kundunFour, kundunFive, jackpot };

        foreach (var monster in gameConfiguration.Monsters)
        {
            foreach (var group in configuredGroups)
            {
                monster.DropItemGroups.Remove(group);
            }
        }

        var regularMonsters = gameConfiguration.Maps
            .SelectMany(map => map.MonsterSpawns)
            .Where(spawn => spawn is { SpawnTrigger: SpawnTrigger.Automatic, MonsterDefinition.ObjectKind: NpcObjectKind.Monster })
            .Select(spawn => spawn.MonsterDefinition!)
            .Where(monster => !BossMonsterNumbers.Contains(monster.Number))
            .Distinct()
            .ToList();
        var bosses = gameConfiguration.Monsters
            .Where(monster => monster.ObjectKind == NpcObjectKind.Monster && BossMonsterNumbers.Contains(monster.Number))
            .ToList();
        foreach (var boss in bosses)
        {
            foreach (var obsolete in boss.DropItemGroups.Where(group => group.Chance < 1.0 && !configuredGroups.Contains(group)).ToList())
            {
                boss.DropItemGroups.Remove(obsolete);
            }
        }

        foreach (var monster in regularMonsters)
        {
            AttachGroups(monster, jewelGroup, kundunOne, kundunTwo, kundunThree);
            ReserveChanceDropSlot(monster);
        }

        foreach (var monster in bosses)
        {
            AttachGroups(monster, kundunFour, kundunFive, jackpot);
            ReserveChanceDropSlot(monster);
        }
    }

    private static void ConfigureJackpotOpening(IContext context, GameConfiguration gameConfiguration, ItemDefinition kundunBox)
    {
        var gift = GetItemDefinition(gameConfiguration, 14, 52);
        var jackpotId = GuidHelper.CreateGuid<ItemDropItemGroup>(gift.Group, gift.Number, 0);
        var jackpot = gift.DropItems.FirstOrDefault(group => group.GetId() == jackpotId);
        if (jackpot is null)
        {
            jackpot = context.CreateNew<ItemDropItemGroup>();
            jackpot.SetGuid(jackpotId);
            gift.DropItems.Add(jackpot);
        }

        jackpot.SourceItemLevel = 0;
        jackpot.ItemType = SpecialItemType.FullExcellent;
        jackpot.Chance = 1.0;
        jackpot.MinimumLevel = 13;
        jackpot.MaximumLevel = 13;
        jackpot.Description = "Full Option Gacha Box (GM Gift)";
        jackpot.DropEffect = ItemDropEffect.FanfareSound;
        jackpot.PossibleItems.Clear();
        foreach (var item in kundunBox.DropItems.Single(group => group.SourceItemLevel == 12 && group.ItemType is SpecialItemType.Excellent or SpecialItemType.ExcellentWithLuck).PossibleItems)
        {
            jackpot.PossibleItems.Add(item);
        }
    }

    private static DropItemGroup UpsertGachaGroup(
        IContext context,
        GameConfiguration gameConfiguration,
        short tier,
        string description,
        double chance,
        ItemDefinition carrier,
        byte itemLevel)
    {
        var id = GuidHelper.CreateGuid<DropItemGroup>(9_999, tier);
        var group = gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == id);
        if (group is null)
        {
            group = context.CreateNew<DropItemGroup>();
            group.SetGuid(id);
            gameConfiguration.DropItemGroups.Add(group);
        }

        group.Description = description;
        group.Chance = chance;
        group.ItemType = SpecialItemType.RandomItem;
        group.ItemLevel = itemLevel;
        group.MinimumMonsterLevel = null;
        group.MaximumMonsterLevel = null;
        group.Monster = null;
        group.PossibleItems.Clear();
        group.PossibleItems.Add(carrier);
        return group;
    }

    private static void RemoveLegacyKundunMonsterDrops(GameConfiguration gameConfiguration, ItemDefinition kundunBox)
    {
        var legacyGroups = gameConfiguration.DropItemGroups
            .Where(group => group.Monster is not null
                            && group.ItemLevel is >= 8 and <= 12
                            && group.PossibleItems.Count == 1
                            && group.PossibleItems.Single() == kundunBox)
            .ToList();
        foreach (var group in legacyGroups)
        {
            group.Monster?.DropItemGroups.Remove(group);
            gameConfiguration.DropItemGroups.Remove(group);
        }
    }

    private static void AttachGroups(MonsterDefinition monster, params DropItemGroup[] groups)
    {
        foreach (var group in groups)
        {
            if (!monster.DropItemGroups.Contains(group))
            {
                monster.DropItemGroups.Add(group);
            }
        }
    }

    private static void ReserveChanceDropSlot(MonsterDefinition monster)
    {
        monster.NumberOfMaximumItemDrops = 2;
    }

    private static ItemDefinition GetItemDefinition(GameConfiguration gameConfiguration, int group, int number)
        => gameConfiguration.Items.First(item => item.Group == group && item.Number == number);

    private sealed class MerchantStorePacker
    {
        private readonly bool[,] _occupied = new bool[InventoryConstants.WarehouseRows, InventoryConstants.RowSize];
        private readonly MonsterDefinition _merchant;
        private readonly List<Item> _pendingItems = [];

        internal MerchantStorePacker(ItemStorage store, MonsterDefinition merchant)
        {
            _ = new Storage(InventoryConstants.WarehouseSize, store);
            this.Store = store;
            this._merchant = merchant;
        }

        internal ItemStorage Store { get; }

        internal void Add(Item item)
        {
            this._pendingItems.Add(item);
        }

        internal void Complete()
        {
            foreach (var item in this._pendingItems.OrderByDescending(item => item.Definition?.Height).ThenByDescending(item => item.Definition?.Width))
            {
                if (item.Definition is null || !this.TryReserve(item.Definition, out var slot))
                {
                    throw new InvalidOperationException($"Merchant {this._merchant} cannot fit item {item.Definition}.");
                }

                item.ItemSlot = slot;
                this.Store.Items.Add(item);
            }
        }

        private bool TryReserve(ItemDefinition definition, out byte slot)
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
