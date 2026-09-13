// <copyright file="ConfigureInstantWingCraftingUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Configures the instant-server wing mixes and their result options.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("0785B9DE-AA4C-4AEC-8EFF-DF85CB9FE4F6")]
public sealed class ConfigureInstantWingCraftingUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure Instant Wing Crafting";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Sets wing mixes to 90%, guarantees Luck and a maximum normal option, and gives each supported special wing line a 90% roll.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureInstantWingCrafting;

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
        InstantServerConfiguration.ConfigureWingCraftings(gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
