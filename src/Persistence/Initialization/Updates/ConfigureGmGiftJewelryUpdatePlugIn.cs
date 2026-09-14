// <copyright file="ConfigureGmGiftJewelryUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the low-rate full-excellent jewelry reward to GM Gift.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("8C59E255-4498-46B3-B867-C2AE00DD4B4F")]
public sealed class ConfigureGmGiftJewelryUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The update name.
    /// </summary>
    internal const string PlugInName = "Configure GM Gift Jewelry";

    /// <summary>
    /// The update description.
    /// </summary>
    internal const string PlugInDescription = "Adds a 1% chance for GM Gift to open as a level +4 full-excellent ring or pendant.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.ConfigureGmGiftJewelry;

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

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        InstantServerConfiguration.ConfigureGmGiftJewelry(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }
}
