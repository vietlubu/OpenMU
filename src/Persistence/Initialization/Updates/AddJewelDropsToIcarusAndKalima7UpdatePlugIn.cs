// <copyright file="AddJewelDropsToIcarusAndKalima7UpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates Icarus and Kalima 7 map drops, updates Kundun drops, and adds Elphis to Noria.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("E3812FC2-E2DD-450C-868C-0DF0444C9194")]
public sealed class AddJewelDropsToIcarusAndKalima7UpdatePlugIn : UpdatePlugInBase
{
    internal const string PlugInName = "Add Jewel Drops to Icarus and Kalima 7";

    internal const string PlugInDescription = "Adds Gemstone and Jewel of Guardian drops to Icarus and Kalima 7. Updates Boss Kundun drops and clones Elphis to Noria.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddJewelDropsToIcarusAndKalima7;

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
        var icarusMap = gameConfiguration.Maps.First(m => m.Number == 10);
        var kalima7Map = gameConfiguration.Maps.First(m => m.Number == 36);
        var noriaMap = gameConfiguration.Maps.First(m => m.Number == 3);
        
        var gemstone = gameConfiguration.Items.First(item => item.Group == 14 && item.Number == 41);
        var jog = gameConfiguration.Items.First(item => item.Group == 14 && item.Number == 31);
        var harmony = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 42);

        // Icarus Drops
        var icarusGemstoneGroup = context.CreateNew<DropItemGroup>();
        icarusGemstoneGroup.SetGuid(icarusMap.Number, 3);
        icarusGemstoneGroup.Chance = 0.03;
        icarusGemstoneGroup.Description = "Gemstone";
        icarusGemstoneGroup.PossibleItems.Add(gemstone);
        icarusMap.DropItemGroups.Add(icarusGemstoneGroup);
        gameConfiguration.DropItemGroups.Add(icarusGemstoneGroup);

        var icarusJogGroup = context.CreateNew<DropItemGroup>();
        icarusJogGroup.SetGuid(icarusMap.Number, 4);
        icarusJogGroup.Chance = 0.03;
        icarusJogGroup.Description = "Jewel of Guardian";
        icarusJogGroup.PossibleItems.Add(jog);
        icarusMap.DropItemGroups.Add(icarusJogGroup);
        gameConfiguration.DropItemGroups.Add(icarusJogGroup);

        // Kalima 7 Drops
        var kalima7GemstoneGroup = context.CreateNew<DropItemGroup>();
        kalima7GemstoneGroup.SetGuid(kalima7Map.Number, 1);
        kalima7GemstoneGroup.Chance = 0.30;
        kalima7GemstoneGroup.Description = "Gemstone";
        kalima7GemstoneGroup.PossibleItems.Add(gemstone);
        kalima7Map.DropItemGroups.Add(kalima7GemstoneGroup);
        gameConfiguration.DropItemGroups.Add(kalima7GemstoneGroup);

        var kalima7JogGroup = context.CreateNew<DropItemGroup>();
        kalima7JogGroup.SetGuid(kalima7Map.Number, 2);
        kalima7JogGroup.Chance = 0.10;
        kalima7JogGroup.Description = "Jewel of Guardian";
        kalima7JogGroup.PossibleItems.Add(jog);
        kalima7Map.DropItemGroups.Add(kalima7JogGroup);
        gameConfiguration.DropItemGroups.Add(kalima7JogGroup);

        // Kundun Drops
        var kundun = gameConfiguration.Monsters.First(m => m.Number == 275);
        kundun.NumberOfMaximumItemDrops = 6;
        for (short i = 0; i < 5; i++)
        {
            var harmonyDrop = context.CreateNew<DropItemGroup>();
            harmonyDrop.SetGuid(kundun.Number, (short)(10 + i));
            harmonyDrop.Chance = 1.0;
            harmonyDrop.Description = $"Jewel of Harmony {i + 1}";
            harmonyDrop.PossibleItems.Add(harmony);
            kundun.DropItemGroups.Add(harmonyDrop);
            gameConfiguration.DropItemGroups.Add(harmonyDrop);
        }

        // Noria Elphis
        var elphisDef = gameConfiguration.Monsters.First(m => m.Number == 368);
        var elphisSpawn = context.CreateNew<MonsterSpawnArea>();
        elphisSpawn.SetGuid(noriaMap.Number, elphisDef.Number, 15);
        elphisSpawn.GameMap = noriaMap;
        elphisSpawn.MonsterDefinition = elphisDef;
        elphisSpawn.SpawnTrigger = SpawnTrigger.Automatic;
        elphisSpawn.Quantity = 1;
        elphisSpawn.Direction = Direction.SouthEast;
        elphisSpawn.X1 = 175;
        elphisSpawn.X2 = 175;
        elphisSpawn.Y1 = 120;
        elphisSpawn.Y2 = 120;
        noriaMap.MonsterSpawns.Add(elphisSpawn);

        return ValueTask.CompletedTask;
    }
}
