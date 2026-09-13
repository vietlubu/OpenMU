// <copyright file="InstantServerBloodCastleTicketCrafting.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Craftings;

/// <summary>
/// Blood Castle ticket crafting with the instant-server success rate.
/// </summary>
public class InstantServerBloodCastleTicketCrafting : BloodCastleTicketCrafting
{
    /// <inheritdoc />
    protected override byte GetSuccessRate(int eventLevel) => 100;
}
