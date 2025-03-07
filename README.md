# R.E.P.O Essentials

An essential mod for R.E.P.O, providing crucial features to enhance your gameplay experience.

## Compatibility

-   **R.E.P.O.**: v0.1.2
    -   **App ID**: 3241660
    -   **Build ID**: 17560228
-   **BepInEx**: v5.4.2100

## Patches

-   **Currency Culture**: Fixes the in-game currency display that was hardcoded to German locale (which uses decimal points as thousand separators). It now uses your system's culture settings for proper number formatting.
-   **Single-Player Chat**: Enables access to the chat window in single-player mode by tricking the game into thinking you're in multiplayer. The chat window is normally disabled entirely in single player, but this patch allows you to open it and use it for command testing.
    -   **NOTE**: Text-to-speech functionality is not currently available as it would require a decent amount of modification to the game's networking layer. This may be added in the future but isn't high priority.
-   **Chat Character Limit**: Adjusts the maximum number of characters allowed in chat messages. The default game limit is 50 characters, but this patch makes it configurable through the BepInEx config file.
    -   **Config Path**: `BepInEx/config/org.ccarrmcmahon.plugins.repo.essentials.cfg`
    -   **Setting**: `[Chat] CharacterLimit = 50` (default value, can be modified as desired)
    -   **NOTE**: This is a client side adjustment for now as there is no default support for this as a server configuration.

## Building

To build a release package:

1. Run `.\build.ps1` from PowerShell
2. Enter the plugin version, game version, and game build ID when prompted
3. The script will update all version numbers, build the project, and create a release package
