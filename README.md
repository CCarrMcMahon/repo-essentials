# R.E.P.O Essentials

An essential mod for R.E.P.O, providing crucial features to enhance your gameplay experience.

## Compatibility

-   **R.E.P.O.**: v0.1.2
    -   **App ID**: 3241660
    -   **Build ID**: 17560228
-   **BepInEx**: v5.4.2100

## Features

-   **Currency Formatting Fix**: Fixes the in-game currency display that was hardcoded to German locale (which uses decimal points as thousand separators). It now uses your system's culture settings for proper number formatting.

## Building

To build a release package:

1. Run `.\build.ps1` from PowerShell
2. Enter the plugin version, game version, and game build ID when prompted
3. The script will update all version numbers, build the project, and create a release package
