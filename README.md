# Essentials

Essentials is a quality-of-life enhancement mod for the game R.E.P.O. This BepInEx plugin addresses several usability issues in the base game and provides customization options that improve the overall player experience.

The mod focuses on fixing common frustrations and adding missing functionality that players expect, such as proper currency formatting, improved chat functionality, and single-player convenience features. Whether you're playing solo or with friends, these essential improvements make the game more accessible and enjoyable without altering core gameplay mechanics.

Development is ongoing as I try to make R.E.P.O. an even better experience, especially when playing with friends. If you have ideas for new features or improvements, please reach out. I'd love to hear from you as I expect this project to grow based on community feedback. Thanks for checking this plugin out and enjoy the game!

## Compatibility

-   **R.E.P.O.**: v0.1.2
    -   **App ID**: 3241660
    -   **Build ID**: 17560228
-   **BepInEx**: v5.4.2100

## Incompatibilities

This plugin will not work with the following mods and has been configured to avoid loading when they are detected:

-   **NoLimitChatbox** by **nickklmao**: Both plugins modify the chat character limit system in different ways. NoLimitChatbox removes the character limit entirely, while Essentials replaces it with a configurable value.
-   **MorePlayers** by **zelofi**: Both plugins attempt to modify the maximum player limit in the game. The Server Max Players feature in Essentials conflicts with the functionality provided by MorePlayers.

## Patches

### Currency Culture

Fixes the in-game currency display that was hardcoded to German locale (which uses decimal points as thousand separators). It now uses your system's culture settings for proper number formatting.

### Chat Character Limit

Enhances the chat system with multiple customization options, allowing you to adjust how chat appears and functions in-game.

-   **Config Path**: `BepInEx/config/org.ccarrmcmahon.plugins.repo.essentials.cfg`
-   **Settings**:
    -   `[Chat] CharacterLimit = 250` - The maximum number of characters allowed in chat messages (default: 250)
    -   `[Chat] ChatTextWidth = 525` - The width of the chat area in pixels (default: 525)
    -   `[Chat] CharacterSpacing = -0.5` - The spacing between characters in chat (default: -0.5)
    -   `[Chat] LineSpacing = -60` - The spacing between lines in chat (default: -60)
-   **_NOTE_**: These are client-side adjustments as there is no default support for these as server configurations.

### Server Max Players

Allows you to increase the maximum number of players beyond the game's hardcoded limit of 6. You can configure the server to support up to 20 players, making it ideal for larger groups wanting to play together.

-   **Config Path**: `BepInEx/config/org.ccarrmcmahon.plugins.repo.essentials.cfg`
-   **Settings**:
    -   `[Server] MaxPlayers = 6` - The maximum number of players allowed to join a server (default: 6, range: 1-20)

### Single-Player Chat

Enables access to the chat window in single-player mode by tricking the game into thinking you're in multiplayer. The chat window is normally disabled entirely in single player, but this patch allows you to open it and use it for command testing.

-   **_NOTE_**: Text-to-speech functionality is not currently available as it would require a decent amount of modification to the game's networking layer. This may be added in the future but isn't high priority.

## Building

To build a release package:

1. Run `.\build.ps1` from PowerShell
2. Enter the plugin version, game version, and game build ID when prompted
3. The script will update all version numbers, build the project, and create a release package
