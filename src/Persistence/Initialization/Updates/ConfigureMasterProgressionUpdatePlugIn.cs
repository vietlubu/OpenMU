// <copyright file="ConfigureMasterProgressionUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the master progression and final-class command of the Season 6 instant server.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("0F21BD51-BEB2-4B71-976F-3EE79F24A049")]
public sealed class ConfigureMasterProgressionUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure Master Progression";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Raises master progression to 400 levels at x1000 experience with 5 points per level and enables /endclass for game masters.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureMasterProgression;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 09, 13, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureMasterProgression(context, gameConfiguration);

        if (gameConfiguration.PlugInConfigurations.All(configuration => configuration.TypeId != typeof(EndClassChatCommandPlugIn).GUID))
        {
            var configuration = context.CreateNew<PlugInConfiguration>();
            configuration.TypeId = typeof(EndClassChatCommandPlugIn).GUID;
            configuration.IsActive = true;
            gameConfiguration.PlugInConfigurations.Add(configuration);
        }

        return ValueTask.CompletedTask;
    }
}
