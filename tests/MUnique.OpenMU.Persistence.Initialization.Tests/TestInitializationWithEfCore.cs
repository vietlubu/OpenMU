// <copyright file="TestInitializationWithEfCore.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
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
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// The main program class.
/// </summary>
[TestFixture]
internal class TestInitializationWithEfCore
{
    private const byte IcarusMapNumber = 10;
    private static readonly Guid FeatherDropGroupId = new(0x200, IcarusMapNumber, 1, 0, 0, 0, 0, 0, 0, 0, 0);
    private static readonly Guid CrestDropGroupId = new(0x200, IcarusMapNumber, 2, 0, 0, 0, 0, 0, 0, 0, 0);

    /// <summary>
    /// Tests the data initialization using the entity framework core.
    /// </summary>
    [Test]
    [Ignore("This is not a real test which should run automatically.")]
    public async Task SetupDatabaseAndTestLoadingDataAsync()
    {
        var manager = new PersistenceContextProvider(new NullLoggerFactory(), null);
        using var update = await manager.ReCreateDatabaseAsync().ConfigureAwait(false);
        await this.TestDataInitializationAsync(new PersistenceContextProvider(new NullLoggerFactory(), null)).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task TestDataInitializationInMemoryAsync()
    {
        await this.TestDataInitializationAsync(new InMemoryPersistenceContextProvider()).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task TestSeason6DataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.AssertIcarusFeatherAndCrestDropGroupsAsync(contextProvider).ConfigureAwait(false);
        await this.AssertCastleSiegeUpdatePlugInAsync(contextProvider).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
        await this.AssertInstantServerConfigurationAsync(contextProvider).ConfigureAwait(false);
    }

    private async Task AssertInstantServerConfigurationAsync(IPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewConfigurationContext();
        var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var servers = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(false)).ToList();
        var permanentMonsterSpawns = configuration.Maps
            .SelectMany(map => map.MonsterSpawns)
            .Where(spawn => spawn is { SpawnTrigger: SpawnTrigger.Automatic, MonsterDefinition.ObjectKind: NpcObjectKind.Monster })
            .ToList();
        var monsters = configuration.Monsters.Where(monster => monster.ObjectKind == NpcObjectKind.Monster).ToList();

        foreach (var npcNumber in new short[] { 230, 242, 243, 245, 246, 251, 253, 254, 259, 376, 377, 415, 416, 417, 545, 577 })
        {
            var store = configuration.Monsters.Single(monster => monster.Number == npcNumber).MerchantStore;
            Assert.That(store, Is.Not.Null, $"NPC {npcNumber}");
            Assert.DoesNotThrow(() => _ = new Storage(InventoryConstants.WarehouseSize, store!), $"NPC {npcNumber}");
        }

        Assert.Multiple(() =>
        {
            Assert.That(configuration.ExperienceRate, Is.EqualTo(9999f));
            Assert.That(configuration.AreaSkillHitsPlayer, Is.True);
            Assert.That(configuration.ExcellentItemDropLevelDelta, Is.Zero);
            Assert.That(configuration.GlobalBaseAttributeValues.Single(attribute => attribute.Definition?.Id == Stats.MoneyAmountRate.Id).Value, Is.EqualTo(1_000f));
            Assert.That(servers, Is.Not.Empty);
            Assert.That(servers.All(server => server is { ExperienceRate: 1.0f, PvpEnabled: true }), Is.True);
            Assert.That(configuration.CharacterClasses.SelectMany(characterClass => characterClass.StatAttributes).Where(attribute => attribute.Attribute == Stats.PointsPerLevelUp).All(attribute => attribute.BaseValue == 500f), Is.True);
            Assert.That(new[] { Stats.BaseStrength, Stats.BaseAgility, Stats.BaseVitality, Stats.BaseEnergy, Stats.BaseLeadership }.All(stat => configuration.Attributes.Single(attribute => attribute == stat).MaximumValue == 32_767), Is.True);
            Assert.That(permanentMonsterSpawns, Is.Not.Empty);
            Assert.That(permanentMonsterSpawns.All(spawn => spawn.Quantity >= 10), Is.True);
            Assert.That(monsters.All(monster => monster.NumberOfMaximumItemDrops >= 4 && monster.RespawnDelay <= TimeSpan.FromSeconds(5)), Is.True);
            Assert.That(configuration.MiniGameDefinitions.All(miniGame => miniGame.ArePlayerKillersAllowedToEnter), Is.True);
        });

        this.AssertEquipmentProfile(configuration, 254, [0, 2, 3], 2, [(5, 0), (5, 2)]);
        this.AssertEquipmentProfile(configuration, 251, [4, 6, 7], 5, [(0, 5), (0, 6)]);
        this.AssertEquipmentProfile(configuration, 251, [12, 13], 15, [(0, 5), (5, 0)]);
        this.AssertEquipmentProfile(configuration, 251, [16, 17], 25, [(2, 8), (2, 9)]);
        this.AssertEquipmentProfile(configuration, 251, [24, 25], 59, [(0, 32), (0, 33)]);
        this.AssertEquipmentProfile(configuration, 243, [8, 10, 11], 10, [(4, 0), (4, 3)]);
        this.AssertEquipmentProfile(configuration, 416, [20, 22, 23], 40, [(5, 15), (5, 21), (5, 22)]);
        this.AssertSkillProfile(configuration, 254, [0, 2, 3, 12, 13]);
        this.AssertSkillProfile(configuration, 230, [4, 6, 7, 16, 17, 24, 25]);
        this.AssertSkillProfile(configuration, 242, [8, 10, 11]);
        this.AssertSkillProfile(configuration, 417, [20, 22, 23]);

        foreach (var npcNumber in new short[] { 253, 259, 376, 377, 415, 545, 577 })
        {
            var items = configuration.Monsters.Single(monster => monster.Number == npcNumber).MerchantStore!.Items;
            Assert.Multiple(() =>
            {
                Assert.That(items.Single(item => item.Definition is { Group: 14, Number: 3 }).Durability, Is.EqualTo(255));
                Assert.That(items.Single(item => item.Definition is { Group: 14, Number: 6 }).Durability, Is.EqualTo(255));
                Assert.That(items.Select(item => (item.Definition!.Group, item.Definition.Number)), Does.Contain(((byte)14, (short)8)));
                Assert.That(items.Select(item => (item.Definition!.Group, item.Definition.Number)), Does.Contain(((byte)4, (short)7)));
                Assert.That(items.Select(item => (item.Definition!.Group, item.Definition.Number)), Does.Contain(((byte)4, (short)15)));
                Assert.That(items.Select(item => (item.Definition!.Group, item.Definition.Number)), Does.Contain(((byte)14, (short)10)));
                Assert.That(items.Select(item => (item.Definition!.Group, item.Definition.Number)), Does.Contain(((byte)13, (short)29)));
            });
        }

        Assert.That(configuration.Items.Single(item => item is { Group: 14, Number: 3 }).Durability, Is.EqualTo(byte.MaxValue));
        Assert.That(configuration.Items.Single(item => item is { Group: 14, Number: 6 }).Durability, Is.EqualTo(byte.MaxValue));
        var kundunBox = configuration.Items.Single(item => item is { Group: 14, Number: 11 });
        foreach (var level in Enumerable.Range(8, 5).Select(level => (byte)level))
        {
            Assert.That(kundunBox.DropItems.Where(group => group.SourceItemLevel == level), Has.Exactly(1).Items);
            Assert.That(kundunBox.DropItems.Single(group => group.SourceItemLevel == level), Has.Property(nameof(DropItemGroup.Chance)).EqualTo(1.0));
            Assert.That(kundunBox.DropItems.Single(group => group.SourceItemLevel == level).ItemType, Is.EqualTo(SpecialItemType.Excellent));
        }

        var jackpot = configuration.Items.Single(item => item is { Group: 14, Number: 52 }).DropItems.Single();
        Assert.Multiple(() =>
        {
            Assert.That(jackpot.ItemType, Is.EqualTo(SpecialItemType.FullExcellent));
            Assert.That(jackpot.Chance, Is.EqualTo(1.0));
            Assert.That(jackpot.MinimumLevel, Is.EqualTo(13));
            Assert.That(jackpot.MaximumLevel, Is.EqualTo(13));
            Assert.That(jackpot.PossibleItems, Is.EquivalentTo(kundunBox.DropItems.Single(group => group.SourceItemLevel == 12).PossibleItems));
        });

        var gachaGroups = Enumerable.Range(1, 6)
            .Select(tier => configuration.DropItemGroups.Single(group => group.GetId() == new Guid(0x200, 9_999, (short)tier, 0, 0, 0, 0, 0, 0, 0, 0)))
            .ToList();
        Assert.That(gachaGroups.Select(group => group.Chance), Is.EqualTo(new[] { 0.02, 0.015, 0.01, 0.475, 0.475, 0.05 }));
        Assert.That(gachaGroups.All(group => group.PossibleItems.Count == 1), Is.True);
        Assert.That(configuration.DropItemGroups.Where(group => group.Monster is not null && group.ItemLevel is >= 8 and <= 12 && group.PossibleItems.Count == 1 && group.PossibleItems.Single() == kundunBox), Is.Empty);

        var bossNumbers = new HashSet<short> { 43, 44, 53, 54, 78, 79, 80, 81, 82, 83, 135, 161, 181, 189, 197, 267, 275, 295, 338, 361, 362, 363, 364, 440, 459 };
        foreach (var boss in monsters.Where(monster => bossNumbers.Contains(monster.Number)))
        {
            Assert.That(boss.DropItemGroups.Intersect(gachaGroups), Is.EquivalentTo(gachaGroups.Skip(3)));
            Assert.That(boss.DropItemGroups.Where(group => group.Chance < 1.0), Is.EquivalentTo(gachaGroups.Skip(3)));
            Assert.That(boss.NumberOfMaximumItemDrops, Is.GreaterThanOrEqualTo(5 + boss.DropItemGroups.Count(group => group.Chance >= 1.0)));
        }

        var regularMonsters = permanentMonsterSpawns.Select(spawn => spawn.MonsterDefinition!).Where(monster => !bossNumbers.Contains(monster.Number)).Distinct();
        foreach (var monster in regularMonsters)
        {
            Assert.That(monster.DropItemGroups.Intersect(gachaGroups), Is.EquivalentTo(gachaGroups.Take(3)));
            Assert.That(monster.NumberOfMaximumItemDrops, Is.GreaterThanOrEqualTo(5 + monster.DropItemGroups.Count(group => group.Chance >= 1.0)));
        }

        this.AssertContinuousBossEvent<GoldenInvasionPlugIn>(configuration, TimeOnly.MinValue);
        this.AssertContinuousBossEvent<RedDragonInvasionPlugIn>(configuration, new TimeOnly(0, 10));
        this.AssertContinuousBossEvent<WhiteWizardInvasionPlugIn>(configuration, new TimeOnly(0, 20));
    }

    private void AssertEquipmentProfile(GameConfiguration configuration, short npcNumber, byte[] classes, byte setNumber, (byte Group, byte Number)[] weapons)
    {
        var classSet = classes.ToHashSet();
        var storeItems = configuration.Monsters.Single(monster => monster.Number == npcNumber).MerchantStore!.Items;
        var expectedDefinitions = configuration.Items
            .Where(item => (item.Group is >= 7 and <= 11 && item.Number == setNumber || weapons.Contains((item.Group, (byte)item.Number)))
                           && item.QualifiedCharacters.Any(characterClass => classSet.Contains(characterClass.Number)))
            .ToList();
        foreach (var definition in expectedDefinitions)
        {
            var item = storeItems.FirstOrDefault(item => item.Definition == definition);
            Assert.That(item, Is.Not.Null, $"NPC {npcNumber}: {definition}");
            Assert.Multiple(() =>
            {
                Assert.That(item!.Level, Is.EqualTo(7));
                Assert.That(item.ItemOptions.Count(link => link.ItemOption?.OptionType == ItemOptionTypes.Excellent), Is.EqualTo(1));
                Assert.That(item.ItemOptions.Any(link => link.ItemOption?.OptionType == ItemOptionTypes.Luck || link.ItemOption?.OptionType == ItemOptionTypes.Option), Is.False);
                Assert.That(item.ItemOptions.Single(link => link.ItemOption?.OptionType == ItemOptionTypes.Excellent).ItemOption?.PowerUpDefinition?.TargetAttribute, Is.EqualTo(definition.Group >= 7 ? Stats.MaximumHealth : Stats.ExcellentDamageChance));
                Assert.That(item.HasSkill, Is.EqualTo(item.CanHaveSkill()));
            });
        }
    }

    private void AssertSkillProfile(GameConfiguration configuration, short npcNumber, byte[] classes)
    {
        var classSet = classes.ToHashSet();
        var storeItems = configuration.Monsters.Single(monster => monster.Number == npcNumber).MerchantStore!.Items;
        var expectedDefinitions = configuration.Items.Where(item => item.Group is 12 or 15 && item.ItemSlot is null && item.Skill is not null && item.QualifiedCharacters.Any(characterClass => classSet.Contains(characterClass.Number)));
        foreach (var definition in expectedDefinitions)
        {
            Assert.That(storeItems.Any(item => item.Definition == definition), Is.True, $"NPC {npcNumber}: {definition}");
        }

        if (classes.Contains((byte)8))
        {
            Assert.That(storeItems.Where(item => item.Definition is { Group: 12, Number: 11 }).Select(item => item.Level), Is.EquivalentTo(Enumerable.Range(0, 7).Select(level => (byte)level)));
        }
    }

    private void AssertContinuousBossEvent<TPlugIn>(GameConfiguration configuration, TimeOnly firstStart)
        where TPlugIn : SimpleInvasionPlugIn
    {
        var plugIn = configuration.PlugInConfigurations.Single(item => item.TypeId == typeof(TPlugIn).GUID);
        var eventConfiguration = plugIn.GetConfiguration<PeriodicInvasionConfiguration>(null);
        Assert.Multiple(() =>
        {
            Assert.That(plugIn.IsActive, Is.True);
            Assert.That(eventConfiguration, Is.Not.Null);
            Assert.That(eventConfiguration!.PreStartMessageDelay, Is.EqualTo(TimeSpan.Zero));
            Assert.That(eventConfiguration.TaskDuration, Is.EqualTo(TimeSpan.FromMinutes(10)));
            Assert.That(eventConfiguration.Timetable.First(), Is.EqualTo(firstStart));
            Assert.That(eventConfiguration.Timetable.Zip(eventConfiguration.Timetable.Skip(1), (first, second) => second - first).All(interval => interval == TimeSpan.FromMinutes(30)), Is.True);
        });
    }


    /// <summary>
    /// Tests that the instant-server update repairs an existing Season 6 configuration.
    /// </summary>
    [Test]
    public async Task TestInstantServerUpdatePlugInAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using (var context = contextProvider.CreateNewContext())
        {
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var server = (await context.GetAsync<GameServerDefinition>().ConfigureAwait(false)).Single();
            configuration.ExperienceRate = 1f;
            configuration.AreaSkillHitsPlayer = false;
            server.PvpEnabled = false;
            await new ConfigureInstantServerUpdatePlugIn().ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        }

        await this.AssertInstantServerConfigurationAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the completion update is idempotent and repairs persisted character point rates.
    /// </summary>
    [Test]
    public async Task TestCompleteInstantServerUpdatePlugInAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        const int normalFreePoints = 1234;
        const int heroFreePoints = 5678;
        using (var context = contextProvider.CreateNewContext())
        {
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var pointsDefinition = configuration.Attributes.Single(attribute => attribute.Id == Stats.PointsPerLevelUp.Id);
            var heroDefinition = configuration.Attributes.Single(attribute => attribute.Id == Stats.GainHeroStatusQuestCompleted.Id);
            var account = context.CreateNew<Account>();
            account.LoginName = "instant-update";
            var normal = context.CreateNew<Character>();
            normal.Name = "Before117";
            normal.LevelUpPoints = normalFreePoints;
            normal.Attributes.Add(context.CreateNew<StatAttribute>(pointsDefinition, 5));
            account.Characters.Add(normal);
            var hero = context.CreateNew<Character>();
            hero.Name = "Hero117";
            hero.LevelUpPoints = heroFreePoints;
            hero.Attributes.Add(context.CreateNew<StatAttribute>(pointsDefinition, 6));
            hero.Attributes.Add(context.CreateNew<StatAttribute>(heroDefinition, 1));
            account.Characters.Add(hero);

            var update = new CompleteInstantServerUpdatePlugIn();
            await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
            await update.ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        }

        using (var context = contextProvider.CreateNewContext())
        {
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var account = (await context.GetAsync<Account>().ConfigureAwait(false)).Single(item => item.LoginName == "instant-update");
            var normal = account.Characters.Single(character => character.Name == "Before117");
            var hero = account.Characters.Single(character => character.Name == "Hero117");
            Assert.Multiple(() =>
            {
                Assert.That(normal.Attributes.Single(attribute => attribute.Definition == Stats.PointsPerLevelUp).Value, Is.EqualTo(500));
                Assert.That(hero.Attributes.Single(attribute => attribute.Definition == Stats.PointsPerLevelUp).Value, Is.EqualTo(501));
                Assert.That(normal.LevelUpPoints, Is.EqualTo(normalFreePoints));
                Assert.That(hero.LevelUpPoints, Is.EqualTo(heroFreePoints));
                Assert.That(configuration.DropItemGroups.Count(group => group.GetId() == new Guid(0x200, 9_999, 1, 0, 0, 0, 0, 0, 0, 0, 0)), Is.EqualTo(1));
                Assert.That(configuration.Items.Single(item => item is { Group: 14, Number: 52 }).DropItems, Has.Exactly(1).Items);
            });
        }

        await this.AssertInstantServerConfigurationAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the Zen update repairs the multiplier of an existing Season 6 configuration.
    /// </summary>
    [Test]
    public async Task TestIncreaseInstantServerMoneyDropUpdatePlugInAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, false).ConfigureAwait(false);

        using (var context = contextProvider.CreateNewContext())
        {
            var configuration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
            var oldRate = configuration.GlobalBaseAttributeValues.Single(attribute => attribute.Definition?.Id == Stats.MoneyAmountRate.Id);
            configuration.GlobalBaseAttributeValues.Remove(oldRate);
            configuration.GlobalBaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1f, oldRate.Definition));
            await new IncreaseInstantServerMoneyDropUpdatePlugIn().ApplyUpdateAsync(context, configuration).ConfigureAwait(false);
        }

        using var verificationContext = contextProvider.CreateNewConfigurationContext();
        var updatedConfiguration = (await verificationContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        Assert.That(updatedConfiguration.GlobalBaseAttributeValues.Single(attribute => attribute.Definition?.Id == Stats.MoneyAmountRate.Id).Value, Is.EqualTo(1_000f));
    }

    /// <summary>
    /// Tests that applying the update for Crest of Monarch in Season 6 is idempotent.
    /// </summary>
    [Test]
    public async Task TestSeason6CrestOfMonarchUpdatePlugInAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var map = gameConfiguration.Maps.First(m => m.Number == IcarusMapNumber && m.Discriminator == 0);

        if (gameConfiguration.DropItemGroups.FirstOrDefault(group => group.GetId() == CrestDropGroupId) is { } existingCrestGroup)
        {
            map.DropItemGroups.Remove(existingCrestGroup);
            gameConfiguration.DropItemGroups.Remove(existingCrestGroup);
        }

        var update = new AddCrestOfMonarchDropGroupUpdateSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var groups = gameConfiguration.DropItemGroups.Where(group => group.GetId() == CrestDropGroupId).ToList();
        Assert.That(groups, Has.Count.EqualTo(1));
        Assert.That(map.DropItemGroups.Count(group => group.GetId() == CrestDropGroupId), Is.EqualTo(1));
        Assert.That(groups[0].Chance, Is.EqualTo(0.001));
        Assert.That(groups[0].MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(groups[0].ItemLevel, Is.EqualTo((byte)1));
        Assert.That(groups[0].PossibleItems, Has.Count.EqualTo(1));
        Assert.That(groups[0].PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(groups[0].PossibleItems.Single().Number, Is.EqualTo((short)14));
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task Test075DataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version075.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests the data initialization using the in-memory persistence.
    /// </summary>
    [Test]
    public async Task Test095dDataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new Version095d.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        await this.TestIfItemsFitIntoInventoriesAsync(contextProvider).ConfigureAwait(false);
    }

    private async Task TestDataInitializationAsync(IPersistenceContextProvider contextProvider)
    {
        var initialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await initialization.CreateInitialDataAsync(3, true).ConfigureAwait(false);

        // Loading game configuration
        using var context = contextProvider.CreateNewConfigurationContext();
        var gameConfiguraton = (await context.GetAsync<DataModel.Configuration.GameConfiguration>().ConfigureAwait(false)).FirstOrDefault();
        Assert.That(gameConfiguraton, Is.Not.Null);

        await this.AssertCastleSiegeDataAsync(contextProvider).ConfigureAwait(false);

        // Testing loading of an account
        using var accountContext = contextProvider.CreateNewPlayerContext(gameConfiguraton!);
        var account1 = await accountContext.GetAccountByLoginNameAsync("test1", "test1").ConfigureAwait(false);
        Assert.That(account1, Is.Not.Null);
        Assert.That(account1!.LoginName, Is.EqualTo("test1"));
    }

    private async Task TestIfItemsFitIntoInventoriesAsync(IPersistenceContextProvider contextProvider)
    {
        using var configContext = contextProvider.CreateNewConfigurationContext();
        var config = (await configContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        using var context = contextProvider.CreateNewPlayerContext(config);
        var characters = (await context.GetAccountsOrderedByLoginNameAsync(0, 100).ConfigureAwait(false)).SelectMany(a => a.Characters).ToList();
        Assert.That(characters, Is.Not.Empty);
        byte inventorySize = (byte)(InventoryConstants.EquippableSlotsCount + 64);
        foreach (var character in characters)
        {
            try
            {
                var storage = character.Inventory!;
                var inventory = new Storage(inventorySize, InventoryConstants.EquippableSlotsCount, 0, storage);
                Assert.That(inventory.Items.Count(), Is.EqualTo(storage.Items.Count));
            }
            catch (Exception ex)
            {
                Assert.Warn($"{ex.Message} Character: {character.Name}");
            }
        }
    }

    private async Task AssertIcarusFeatherAndCrestDropGroupsAsync(IPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewConfigurationContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var map = gameConfiguration.Maps.First(m => m.Number == IcarusMapNumber && m.Discriminator == 0);

        var featherGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == FeatherDropGroupId);
        var crestGroup = gameConfiguration.DropItemGroups.Single(group => group.GetId() == CrestDropGroupId);

        Assert.That(map.DropItemGroups.Count(group => group.GetId() == FeatherDropGroupId), Is.EqualTo(1));
        Assert.That(map.DropItemGroups.Count(group => group.GetId() == CrestDropGroupId), Is.EqualTo(1));

        Assert.That(featherGroup.Chance, Is.EqualTo(0.001));
        Assert.That(featherGroup.MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(featherGroup.ItemLevel, Is.Null);
        Assert.That(featherGroup.PossibleItems, Has.Count.EqualTo(1));
        Assert.That(featherGroup.PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(featherGroup.PossibleItems.Single().Number, Is.EqualTo((short)14));

        Assert.That(crestGroup.Chance, Is.EqualTo(0.001));
        Assert.That(crestGroup.MinimumMonsterLevel, Is.EqualTo((byte)82));
        Assert.That(crestGroup.ItemLevel, Is.EqualTo((byte)1));
        Assert.That(crestGroup.PossibleItems, Has.Count.EqualTo(1));
        Assert.That(crestGroup.PossibleItems.Single().Group, Is.EqualTo((byte)13));
        Assert.That(crestGroup.PossibleItems.Single().Number, Is.EqualTo((short)14));
    }

    private async Task AssertCastleSiegeDataAsync(IPersistenceContextProvider contextProvider)
    {
        using var configurationContext = contextProvider.CreateNewConfigurationContext();
        var gameConfiguration = (await configurationContext.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var configuration = gameConfiguration.CastleSiegeConfiguration;
        Assert.That(configuration, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(configuration!.Enabled, Is.True);
            Assert.That(configuration.CrownHoldTimeSeconds, Is.EqualTo(30));
            Assert.That(configuration.RegisterMinLevel, Is.EqualTo(200));
            Assert.That(configuration.RegisterMinMembers, Is.EqualTo(20));
            Assert.That(configuration.ParticipantRewardMinSeconds, Is.EqualTo(60));
            Assert.That(configuration.MaxAttackingGuilds, Is.EqualTo(3));
            Assert.That(configuration.GateBuyPrice, Is.EqualTo(9_500_000));
            Assert.That(configuration.StatueBuyPrice, Is.EqualTo(4_500_000));
            Assert.That(configuration.GateRepairCostPerHealthPoint, Is.EqualTo(5));
            Assert.That(configuration.StatueRepairCostPerHealthPoint, Is.EqualTo(3));
            Assert.That(configuration.RepairCostPerUpgradeLevel, Is.EqualTo(1_000_000));
            Assert.That(configuration.CastleSiegeMapDefinition?.Number, Is.EqualTo(30));
            Assert.That(configuration.LandOfTrialsMapDefinition?.Number, Is.EqualTo(31));
            Assert.That(configuration.SignOfLordItemDefinition?.Group, Is.EqualTo(14));
            Assert.That(configuration.SignOfLordItemDefinition?.Number, Is.EqualTo(21));
            Assert.That(configuration.SignOfLordItemDefinition?.MaximumItemLevel, Is.GreaterThanOrEqualTo(3));
            Assert.That(configuration.SignOfLordItemLevel, Is.EqualTo(3));
            Assert.That(configuration.DefenseRespawnArea, Is.Not.Null);
            Assert.That(configuration.AttackRespawnArea, Is.Not.Null);
        });
        Assert.That(
            gameConfiguration.MagicEffects
                .Where(effect => Enum.IsDefined(typeof(CastleSiegeMagicEffectNumber), effect.Number))
                .Select(effect => effect.Number),
            Is.EquivalentTo(Enum.GetValues<CastleSiegeMagicEffectNumber>().Select(number => (short)number)));

        var expectedSchedule = new (CastleSiegeState State, DayOfWeek Day, byte Hour, byte Minute)[]
        {
            (CastleSiegeState.Idle1, DayOfWeek.Sunday, 0, 0),
            (CastleSiegeState.RegisterGuild, DayOfWeek.Monday, 0, 0),
            (CastleSiegeState.Idle2, DayOfWeek.Tuesday, 0, 0),
            (CastleSiegeState.RegisterMark, DayOfWeek.Wednesday, 0, 0),
            (CastleSiegeState.Idle3, DayOfWeek.Thursday, 0, 0),
            (CastleSiegeState.Notify, DayOfWeek.Friday, 0, 0),
            (CastleSiegeState.Ready, DayOfWeek.Saturday, 18, 0),
            (CastleSiegeState.Start, DayOfWeek.Saturday, 20, 0),
            (CastleSiegeState.End, DayOfWeek.Saturday, 22, 0),
            (CastleSiegeState.EndCycle, DayOfWeek.Saturday, 22, 5),
        };
        var actualSchedule = configuration!.StateSchedule
            .Select(entry => (entry.State, entry.DayOfWeek, entry.Hour, entry.Minute));
        Assert.That(actualSchedule, Is.EquivalentTo(expectedSchedule));

        var expectedNpcs = new (short Number, byte Instance, bool Persisted, CastleSiegeJoinSide Side, byte X, byte Y, Direction Direction)[]
        {
            (216, 1, false, CastleSiegeJoinSide.Attack1, 176, 212, Direction.SouthWest),
            (217, 1, false, CastleSiegeJoinSide.Attack1, 167, 194, Direction.NorthWest),
            (218, 1, false, CastleSiegeJoinSide.Attack1, 184, 195, Direction.NorthWest),
            (219, 1, false, CastleSiegeJoinSide.Defense, 93, 208, Direction.SouthWest),
            (219, 2, false, CastleSiegeJoinSide.Defense, 81, 165, Direction.SouthWest),
            (219, 3, false, CastleSiegeJoinSide.Defense, 107, 165, Direction.SouthWest),
            (219, 4, false, CastleSiegeJoinSide.Defense, 67, 118, Direction.SouthWest),
            (219, 5, false, CastleSiegeJoinSide.Defense, 93, 118, Direction.SouthWest),
            (219, 6, false, CastleSiegeJoinSide.Defense, 119, 118, Direction.SouthWest),
            (221, 1, false, CastleSiegeJoinSide.Attack1, 63, 19, Direction.NorthEast),
            (221, 2, false, CastleSiegeJoinSide.Attack1, 119, 19, Direction.NorthEast),
            (222, 1, false, CastleSiegeJoinSide.Defense, 80, 188, Direction.SouthWest),
            (222, 2, false, CastleSiegeJoinSide.Defense, 105, 188, Direction.SouthWest),
            (277, 1, true, CastleSiegeJoinSide.Defense, 93, 204, Direction.SouthWest),
            (277, 2, true, CastleSiegeJoinSide.Defense, 81, 161, Direction.SouthWest),
            (277, 3, true, CastleSiegeJoinSide.Defense, 107, 161, Direction.SouthWest),
            (277, 4, true, CastleSiegeJoinSide.Defense, 67, 114, Direction.SouthWest),
            (277, 5, true, CastleSiegeJoinSide.Defense, 93, 114, Direction.SouthWest),
            (277, 6, true, CastleSiegeJoinSide.Defense, 119, 114, Direction.SouthWest),
            (283, 1, true, CastleSiegeJoinSide.Defense, 94, 227, Direction.SouthWest),
            (283, 2, true, CastleSiegeJoinSide.Defense, 94, 182, Direction.SouthWest),
            (283, 3, true, CastleSiegeJoinSide.Defense, 82, 130, Direction.SouthWest),
            (283, 4, true, CastleSiegeJoinSide.Defense, 107, 130, Direction.SouthWest),
        };
        var actualNpcs = configuration.NpcDefinitions.Select(
            definition =>
                (definition.MonsterDefinition!.Number,
                 definition.InstanceId,
                 definition.IsPersistedToDatabase,
                 definition.DefaultSide,
                 definition.SpawnX,
                 definition.SpawnY,
                 definition.Direction));
        Assert.That(actualNpcs, Is.EquivalentTo(expectedNpcs));
        Assert.That(
            gameConfiguration.Monsters.Select(monster => monster.Number),
            Is.SupersetOf(new short[] { 216, 217, 218, 219, 220, 221, 222, 223, 224, 277, 283 }));

        this.AssertUpgrades(configuration.GateDefenseUpgrades, [(0, 0, 0, 100), (1, 2, 3_000_000, 180), (2, 3, 3_000_000, 300), (3, 4, 3_000_000, 520)]);
        this.AssertUpgrades(configuration.StatueDefenseUpgrades, [(0, 0, 0, 80), (1, 3, 3_000_000, 180), (2, 5, 3_000_000, 340), (3, 7, 3_000_000, 550)]);
        this.AssertUpgrades(configuration.GateLifeUpgrades, [(0, 0, 0, 1_900_000), (1, 2, 1_000_000, 2_500_000), (2, 3, 1_000_000, 3_500_000), (3, 4, 1_000_000, 5_200_000)]);
        this.AssertUpgrades(configuration.StatueLifeUpgrades, [(0, 0, 0, 1_500_000), (1, 3, 1_000_000, 2_200_000), (2, 5, 1_000_000, 3_400_000), (3, 7, 1_000_000, 5_000_000)]);
        this.AssertUpgrades(configuration.StatueRegenUpgrades, [(0, 0, 0, 0), (1, 3, 5_000_000, 1), (2, 5, 5_000_000, 2), (3, 7, 5_000_000, 3)]);

        this.AssertZones(configuration.AttackMachineZones, [(62, 103, 72, 112), (88, 104, 124, 111), (116, 105, 124, 112), (73, 86, 105, 103)]);
        this.AssertZones(configuration.DefenseMachineZones, [(61, 88, 93, 108), (92, 89, 127, 111), (84, 52, 102, 66)]);
        this.AssertZone(configuration.DefenseRespawnArea!, (74, 144, 115, 154));
        this.AssertZone(configuration.AttackRespawnArea!, (35, 11, 144, 48));

        using (var dataContext = contextProvider.CreateNewContext(gameConfiguration))
        {
            var data = (await dataContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
            Assert.Multiple(() =>
            {
                Assert.That(data.OwnerGuildId, Is.Null);
                Assert.That(data.IsOccupied, Is.False);
                Assert.That(data.TaxChaos, Is.Zero);
                Assert.That(data.TaxStore, Is.Zero);
                Assert.That(data.TaxHunt, Is.Zero);
                Assert.That(data.IsHuntZoneEnabled, Is.False);
                Assert.That(data.TributeMoney, Is.Zero);
                Assert.That(data.NpcStates, Has.Count.EqualTo(10));
                Assert.That(data.NpcStates.Count(state => state.MonsterNumber == 277 && state.CurrentHp == 1_900_000), Is.EqualTo(6));
                Assert.That(data.NpcStates.Count(state => state.MonsterNumber == 283 && state.CurrentHp == 1_500_000), Is.EqualTo(4));
            });

            Assert.That(await dataContext.GetAsync<CastleSiegeGuildRegistration>().ConfigureAwait(false), Is.Empty);

            data.TaxStore = 1;
            data.NpcStates.First().CurrentHp--;
            Assert.That(await dataContext.SaveChangesAsync().ConfigureAwait(false), Is.True);
        }

        using var reloadedContext = contextProvider.CreateNewContext(gameConfiguration);
        var reloadedData = (await reloadedContext.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        Assert.That(reloadedData.TaxStore, Is.EqualTo(1));
        Assert.That(reloadedData.NpcStates, Has.One.Matches<CastleSiegeNpcState>(state => state.CurrentHp is 1_899_999 or 1_499_999));
    }

    private async Task AssertCastleSiegeUpdatePlugInAsync(InMemoryPersistenceContextProvider contextProvider)
    {
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).Single();
        var existingConfiguration = gameConfiguration.CastleSiegeConfiguration!;
        var existingData = (await context.GetAsync<CastleSiegeData>().ConfigureAwait(false)).Single();
        gameConfiguration.CastleSiegeConfiguration = null;
        Assert.That(await context.DeleteAsync(existingConfiguration).ConfigureAwait(false), Is.True);
        Assert.That(await context.DeleteAsync(existingData).ConfigureAwait(false), Is.True);

        var update = new AddCastleSiegeDataUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.CastleSiegeConfiguration, Is.Not.Null);
        Assert.That(await context.GetAsync<CastleSiegeData>().ConfigureAwait(false), Has.Exactly(1).Items);

        var configuration = gameConfiguration.CastleSiegeConfiguration!;
        var signOfLord = gameConfiguration.Items.Single(item => item.Group == 14 && item.Number == 21);
        configuration.SignOfLordItemDefinition = null;
        configuration.SignOfLordItemLevel = 0;
        signOfLord.MaximumItemLevel = 0;

        var registrationUpdate = new ConfigureCastleSiegeRegistrationUpdatePlugIn();
        await registrationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await registrationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(configuration.SignOfLordItemDefinition, Is.SameAs(signOfLord));
            Assert.That(configuration.SignOfLordItemLevel, Is.EqualTo(3));
            Assert.That(signOfLord.MaximumItemLevel, Is.EqualTo(3));
        });

        var customSignOfLord = gameConfiguration.Items.First(item => item != signOfLord);
        configuration.SignOfLordItemDefinition = customSignOfLord;
        configuration.SignOfLordItemLevel = 1;
        await registrationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(configuration.SignOfLordItemDefinition, Is.SameAs(customSignOfLord));
            Assert.That(configuration.SignOfLordItemLevel, Is.EqualTo(1));
        });

        gameConfiguration.Items.Remove(signOfLord);
        configuration.SignOfLordItemDefinition = null;
        configuration.SignOfLordItemLevel = 0;
        await registrationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(configuration.SignOfLordItemDefinition, Is.Null);
            Assert.That(configuration.SignOfLordItemLevel, Is.Zero);
        });

        foreach (var participantEffect in gameConfiguration.MagicEffects
                     .Where(effect => Enum.IsDefined(typeof(CastleSiegeMagicEffectNumber), effect.Number))
                     .ToList())
        {
            gameConfiguration.MagicEffects.Remove(participantEffect);
        }

        var participationUpdate = new ConfigureCastleSiegeParticipationUpdatePlugIn();
        await participationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await participationUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(
            gameConfiguration.MagicEffects
                .Where(effect => Enum.IsDefined(typeof(CastleSiegeMagicEffectNumber), effect.Number))
                .Select(effect => effect.Number),
            Is.EquivalentTo(Enum.GetValues<CastleSiegeMagicEffectNumber>().Select(number => (short)number)));

        var senior = gameConfiguration.Monsters.Single(monster => monster.Number == 223);
        Assert.That(senior.NpcWindow, Is.EqualTo(NpcWindow.CastleSeniorNPC));
        senior.NpcWindow = NpcWindow.Undefined;
        var economyUpdate = new ConfigureCastleSiegeEconomyUpdatePlugIn();
        await economyUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await economyUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(senior.NpcWindow, Is.EqualTo(NpcWindow.CastleSeniorNPC));

        var lifeStone = gameConfiguration.Monsters.Single(monster => monster.Number == 278);
        var maximumHealth = lifeStone.Attributes.Single(attribute => attribute.AttributeDefinition?.Id == Stats.MaximumHealth.Id);
        maximumHealth.Value = 12_345;
        var defense = lifeStone.Attributes.Single(attribute => attribute.AttributeDefinition?.Id == Stats.DefenseBase.Id);
        lifeStone.Attributes.Remove(defense);
        var lifeStoneUpdate = new ConfigureCastleSiegeLifeStoneUpdatePlugIn();
        await lifeStoneUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await lifeStoneUpdate.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(maximumHealth.Value, Is.EqualTo(12_345));
            Assert.That(
                lifeStone.Attributes.Single(attribute => attribute.AttributeDefinition?.Id == Stats.DefenseBase.Id).Value,
                Is.Zero);
        });

        gameConfiguration.Items.Add(signOfLord);
        configuration.SignOfLordItemDefinition = signOfLord;
        configuration.SignOfLordItemLevel = 3;
    }

    private void AssertUpgrades(
        IEnumerable<CastleSiegeUpgradeDefinition> actual,
        IEnumerable<(byte Level, int JewelCount, int Zen, int Value)> expected)
    {
        Assert.That(
            actual.Select(upgrade => (upgrade.Level, upgrade.RequiredJewelOfGuardianCount, upgrade.RequiredZen, upgrade.Value)),
            Is.EquivalentTo(expected));
    }

    private void AssertZones(
        IEnumerable<CastleSiegeZoneDefinition> actual,
        IEnumerable<(byte X1, byte Y1, byte X2, byte Y2)> expected)
    {
        Assert.That(actual.Select(zone => (zone.X1, zone.Y1, zone.X2, zone.Y2)), Is.EquivalentTo(expected));
    }

    private void AssertZone(CastleSiegeZoneDefinition actual, (byte X1, byte Y1, byte X2, byte Y2) expected)
    {
        Assert.That((actual.X1, actual.Y1, actual.X2, actual.Y2), Is.EqualTo(expected));
    }
}
