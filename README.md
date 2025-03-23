# Essentials

Essentials is a quality-of-life enhancement mod for the game R.E.P.O. This BepInEx plugin addresses several usability issues in the base game and provides customization options that improve the overall player experience.

The mod focuses on fixing common frustrations and adding missing functionality that players expect, such as proper currency formatting, improved chat functionality, and single-player convenience features. Whether you're playing solo or with friends, these essential improvements make the game more accessible and enjoyable without altering core gameplay mechanics.

Development is ongoing as I try to make R.E.P.O. an even better experience, especially when playing with friends. If you have ideas for new features or improvements, please reach out. I'd love to hear from you as I expect this project to grow based on community feedback. Thanks for checking this plugin out and enjoy the game!

## Table of Contents

-   [Compatibility](#compatibility)
-   [Incompatibilities](#incompatibilities)
-   [Configuration](#configuration)
-   [Patches](#patches)
    -   [Currency Culture](#currency-culture)
    -   [Chat Character Limit](#chat-character-limit)
    -   [Server Max Players](#server-max-players)
    -   [Single-Player Chat](#single-player-chat)
-   [Building](#building)
-   [License](#license)

## Compatibility

-   **R.E.P.O.**: v0.1.2
    -   **App ID**: 3241660
    -   **Build ID**: 17560228
-   **BepInEx**: v5.4.2100

## Incompatibilities

This plugin will not work with the following mods and has been configured to avoid loading when they are detected:

-   **NoLimitChatbox** by **nickklmao**: Both plugins modify the chat character limit system in different ways. NoLimitChatbox removes the character limit entirely, while Essentials replaces it with a configurable value.
-   **MorePlayers** by **zelofi**: This mod alters the maximum player limit as well, creating a direct conflict with Essentials' Server Max Players feature. Both cannot be used simultaneously.

## Configuration

All plugin settings can be found in the following config file:

-   **Config Path**: `BepInEx/config/org.ccarrmcmahon.plugins.repo.essentials.cfg`

## Patches

### Currency Culture

Fixes the in-game currency display that was hardcoded to German locale (which uses decimal points as thousand separators). It now uses your system's culture settings for proper number formatting.

### Chat Character Limit

Enhances the chat system with multiple customization options, allowing you to adjust how chat appears and functions in-game.

| Setting                   | Default | Description                                           |
| ------------------------- | ------- | ----------------------------------------------------- |
| `[Chat] CharacterLimit`   | 250     | Maximum number of characters allowed in chat messages |
| `[Chat] ChatTextWidth`    | 525     | Width of the chat area in pixels                      |
| `[Chat] CharacterSpacing` | -0.5    | Spacing between characters in chat                    |
| `[Chat] LineSpacing`      | -60     | Spacing between lines in chat                         |

**Note**: These are client-side adjustments as there is no default support for these as server configurations.

### Server Max Players

Allows you to increase the maximum number of players beyond the game's hardcoded limit of 6.

| Setting               | Default | Range | Description                                        |
| --------------------- | ------- | ----- | -------------------------------------------------- |
| `[Server] MaxPlayers` | 6       | 1-20  | Maximum number of players allowed to join a server |

### Single-Player Chat

Enables access to the chat window in single-player mode by tricking the game into thinking you're in multiplayer. The chat window is normally disabled entirely in single player, but this patch allows you to open it and use it for command testing.

**Note**: Text-to-speech functionality is not currently available as it would require significant modification to the game's networking layer. This may be added in the future.

## Building

To build a release package:

1. Run `.\build.ps1` from PowerShell
2. Enter the plugin version, game version, and game build ID when prompted
3. The script will update all version numbers, build the project, and create a release package

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
