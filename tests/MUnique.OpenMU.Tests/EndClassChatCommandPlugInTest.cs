// <copyright file="EndClassChatCommandPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Tests for <see cref="EndClassChatCommandPlugIn"/>.
/// </summary>
[TestFixture]
public class EndClassChatCommandPlugInTest
{
    /// <summary>
    /// Verifies that the command follows every class evolution and reconnects the player to rebuild its attributes.
    /// </summary>
    [Test]
    public async ValueTask ChangesToFinalClassAndReconnectsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var character = player.SelectedCharacter!;
        var finalClass = new CharacterClass { IsMasterClass = true };
        var intermediateClass = new CharacterClass { NextGenerationClass = finalClass };
        Mock.Get(character.CharacterClass!).SetupGet(value => value.NextGenerationClass).Returns(intermediateClass);

        await new EndClassChatCommandPlugIn().HandleCommandAsync(player, "/endclass").ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(character.CharacterClass, Is.SameAs(finalClass));
            Assert.That(player.SelectedCharacter, Is.Null);
            Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.Finished));
        });
    }
}
