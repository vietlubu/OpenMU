// <copyright file="EndClassChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A chat command plugin which changes the game master's character to its final class.
/// </summary>
[Guid("7D5AC70D-1747-4DEE-AD6A-27BA57C2BC24")]
[PlugIn]
[Display(Name = "End class command", Description = "Changes your character to its final class and reconnects.")]
[ChatCommandHelp(Command, typeof(EmptyChatCommandArgs), CharacterStatus.GameMaster)]
public class EndClassChatCommandPlugIn : ChatCommandPlugInBase<EmptyChatCommandArgs>
{
    private const string Command = "/endclass";

    /// <inheritdoc />
    public override string Key => Command;

    /// <inheritdoc />
    public override CharacterStatus MinCharacterStatusRequirement => CharacterStatus.GameMaster;

    /// <inheritdoc />
    protected override async ValueTask DoHandleCommandAsync(Player player, EmptyChatCommandArgs arguments)
    {
        if (player.SelectedCharacter?.CharacterClass is not { } characterClass)
        {
            return;
        }

        while (characterClass.NextGenerationClass is { } nextClass)
        {
            characterClass = nextClass;
        }

        if (characterClass == player.SelectedCharacter.CharacterClass)
        {
            return;
        }

        player.SelectedCharacter.CharacterClass = characterClass;
        await player.DisconnectAsync().ConfigureAwait(false);
    }
}
