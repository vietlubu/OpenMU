// <copyright file="ConfigureKalimaSevenUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures Kalima 7 as a high-difficulty end-game map.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("79214C04-4524-4C99-862F-2E49CFC53C4D")]
public sealed class ConfigureKalimaSevenUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure Kalima 7";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Randomizes Kalima 7 monsters across the combat area, respawns players at the entrance, and increases monster strength.";

    /// <summary>
    /// The baseline attributes of Kalima 7's regular monsters.
    /// </summary>
    internal static readonly (short Number, float Health, float MinimumDamage, float MaximumDamage, float Defense, float AttackRate, float DefenseRate)[] RegularMonsterStats =
    [
        (331, 820_000, 71_200, 75_200, 5_840, 18_700, 6_930),
        (332, 880_000, 75_100, 79_100, 6_150, 19_360, 7_260),
        (333, 973_000, 80_500, 84_500, 6_600, 20_240, 7_590),
        (334, 1_100_000, 87_000, 91_500, 7_200, 21_120, 8_140),
        (335, 1_250_000, 95_100, 99_600, 7_830, 22_330, 8_910),
        (336, 1_450_000, 104_000, 108_500, 8_650, 23_760, 9_680),
        (337, 1_700_000, 116_800, 121_300, 9_920, 26_180, 10_670),
    ];

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureKalimaSeven;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 14, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <summary>
    /// Applies the configured regular-monster attributes at the given multiplier.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="multiplier">The attribute multiplier.</param>
    internal static void ApplyRegularMonsterStats(GameConfiguration gameConfiguration, float multiplier)
    {
        foreach (var stats in RegularMonsterStats)
        {
            var monster = gameConfiguration.Monsters.Single(monster => monster.Number == stats.Number);
            monster.RespawnDelay = TimeSpan.FromSeconds(5);
            Set(monster, Stats.MaximumHealth, stats.Health * multiplier);
            Set(monster, Stats.MinimumPhysBaseDmg, stats.MinimumDamage * multiplier);
            Set(monster, Stats.MaximumPhysBaseDmg, stats.MaximumDamage * multiplier);
            Set(monster, Stats.DefenseBase, stats.Defense * multiplier);
            Set(monster, Stats.AttackRatePvm, stats.AttackRate * multiplier);
            Set(monster, Stats.DefenseRatePvm, stats.DefenseRate * multiplier);
        }
    }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var map = gameConfiguration.Maps.Single(map => map is { Number: Kalima7.Number, Discriminator: 0 });
        var regularNumbers = RegularMonsterStats.Select(stats => stats.Number).ToHashSet();
        foreach (var spawn in map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition is { } monster && regularNumbers.Contains(monster.Number)))
        {
            spawn.X1 = 28;
            spawn.X2 = 121;
            spawn.Y1 = 6;
            spawn.Y2 = 109;
            spawn.Quantity = 1;
            spawn.Direction = Direction.Undefined;
        }

        var bossSpawn = map.MonsterSpawns.Single(spawn => spawn.MonsterDefinition?.Number == 275);
        bossSpawn.X1 = 26;
        bossSpawn.X2 = 26;
        bossSpawn.Y1 = 76;
        bossSpawn.Y2 = 76;
        bossSpawn.Quantity = 1;

        var entrance = map.ExitGates.Single(gate => gate is { X1: 10, Y1: 16, X2: 17, Y2: 22 });
        entrance.Direction = Direction.Undefined;
        entrance.IsSpawnGate = true;
        map.SafezoneMap = map;

        ApplyRegularMonsterStats(gameConfiguration, 1);

        var boss = gameConfiguration.Monsters.Single(monster => monster.Number == 275);
        Set(boss, Stats.MaximumHealth, 120_000_000);
        Set(boss, Stats.MinimumPhysBaseDmg, 60_000);
        Set(boss, Stats.MaximumPhysBaseDmg, 72_000);
        Set(boss, Stats.DefenseBase, 60_000);
        Set(boss, Stats.AttackRatePvm, 60_000);
        Set(boss, Stats.DefenseRatePvm, 48_000);
        return ValueTask.CompletedTask;
    }

    private static void Set(MonsterDefinition monster, AttributeDefinition definition, float value)
        => monster.Attributes.Single(attribute => attribute.AttributeDefinition?.Id == definition.Id).Value = value;
}
