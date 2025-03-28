param (
    [Parameter(Mandatory = $false)]
    [string]$PluginVersion = "",
    
    [Parameter(Mandatory = $false)]
    [string]$GameVersion = "",
    
    [Parameter(Mandatory = $false)]
    [string]$BuildID = ""
)

$ErrorActionPreference = "Stop"

# Project root directory
$rootDir = $PSScriptRoot

# Track if versions were provided as parameters
$pluginVersionProvided = -not [string]::IsNullOrEmpty($PluginVersion)
$gameVersionProvided = -not [string]::IsNullOrEmpty($GameVersion)
$buildIDProvided = -not [string]::IsNullOrEmpty($BuildID)

# Get current version values
$currentPluginVersion = ""
$currentGameVersion = ""
$currentBuildID = ""

# Read current plugin version
$pluginInfoPath = Join-Path $rootDir "src\Configs\PluginInfo.cs"
if (Test-Path $pluginInfoPath) {
    $pluginInfoContent = Get-Content $pluginInfoPath -Raw
    if ($pluginInfoContent -match 'public const string VERSION = "([^"]+)";') {
        $currentPluginVersion = $matches[1]
    }
}

# Read current game version and build ID
$gameInfoPath = Join-Path $rootDir "src\Configs\GameInfo.cs"
if (Test-Path $gameInfoPath) {
    $gameInfoContent = Get-Content $gameInfoPath -Raw
    if ($gameInfoContent -match 'public const string VERSION = "([^"]+)";') {
        $currentGameVersion = $matches[1]
    }
    if ($gameInfoContent -match 'public const string BUILD_ID = "([^"]+)";') {
        $currentBuildID = $matches[1]
    }
}

# Ask for versions if not provided as parameters
if (-not $pluginVersionProvided) {
    $userInput = Read-Host "Enter plugin version [$currentPluginVersion]"
    if (-not [string]::IsNullOrEmpty($userInput)) {
        $PluginVersion = $userInput
        $pluginVersionProvided = $true
    }
}

if (-not $gameVersionProvided) {
    $userInput = Read-Host "Enter game version [$currentGameVersion]"
    if (-not [string]::IsNullOrEmpty($userInput)) {
        $GameVersion = $userInput
        $gameVersionProvided = $true
    }
}

if (-not $buildIDProvided) {
    $userInput = Read-Host "Enter game build ID [$currentBuildID]"
    if (-not [string]::IsNullOrEmpty($userInput)) {
        $BuildID = $userInput
        $buildIDProvided = $true
    }
}

# Only update files if there are changes to make
if ($pluginVersionProvided -or $gameVersionProvided -or $buildIDProvided) {
    Write-Host "Updating version information in files..." -ForegroundColor Cyan

    # Update GameInfo.cs if needed
    if ($gameVersionProvided -or $buildIDProvided) {
        $gameInfoPath = Join-Path $rootDir "src\Configs\GameInfo.cs"
        $gameInfoContent = Get-Content $gameInfoPath -Raw
        
        if ($gameVersionProvided) {
            $gameInfoContent = $gameInfoContent -replace 'public const string VERSION = "[^"]+";', "public const string VERSION = `"$GameVersion`";"
        }
        
        if ($buildIDProvided) {
            $gameInfoContent = $gameInfoContent -replace 'public const string BUILD_ID = "[^"]+";', "public const string BUILD_ID = `"$BuildID`";"
        }

        $gameInfoContent | Out-File -FilePath $gameInfoPath -NoNewline
    }

    # Update PluginInfo.cs if needed
    if ($pluginVersionProvided) {
        $pluginInfoPath = Join-Path $rootDir "src\Configs\PluginInfo.cs"
        $pluginInfoContent = Get-Content $pluginInfoPath -Raw
        $pluginInfoContent = $pluginInfoContent -replace 'public const string VERSION = "[^"]+";', "public const string VERSION = `"$PluginVersion`";"
        $pluginInfoContent | Out-File -FilePath $pluginInfoPath -NoNewline

        # Update manifest.json
        $manifestPath = Join-Path $rootDir "manifest.json"
        $manifestContent = Get-Content $manifestPath -Raw | ConvertFrom-Json
        $manifestContent.version_number = $PluginVersion
        $manifestContent | ConvertTo-Json -Depth 10 | Set-Content $manifestPath

        # Update csproj file
        $csprojPath = Join-Path $rootDir "RepoEssentials.csproj"
        $csprojContent = Get-Content $csprojPath -Raw
        $csprojContent = $csprojContent -replace '<Version>[^<]+</Version>', "<Version>$PluginVersion</Version>"
        $csprojContent | Out-File -FilePath $csprojPath -NoNewline
    }
    
    # Update README.md if needed
    $readmePath = Join-Path $rootDir "README.md"
    if (Test-Path $readmePath) {
        $readmeContent = Get-Content $readmePath -Raw
        
        if ($gameVersionProvided) {
            $readmeContent = $readmeContent -replace '\*\*R\.E\.P\.O\.\*\*: v[0-9.]+', "**R.E.P.O.**: v$GameVersion"
        }
        
        if ($buildIDProvided) {
            $readmeContent = $readmeContent -replace '\*\*Build ID\*\*: [0-9]+', "**Build ID**: $BuildID"
        }
        
        $readmeContent | Out-File -FilePath $readmePath -NoNewline
        Write-Host "Updated version information in README.md" -ForegroundColor Cyan
    }
}

Write-Host "Building project..." -ForegroundColor Cyan

# Build the project
dotnet build -c Release

# For release creation, we need a plugin version
if (-not $pluginVersionProvided) {
    # Extract current version from PluginInfo.cs if not provided
    $pluginInfoPath = Join-Path $rootDir "src\Configs\PluginInfo.cs"
    $pluginInfoContent = Get-Content $pluginInfoPath -Raw
    if ($pluginInfoContent -match 'public const string VERSION = "([^"]+)";') {
        $PluginVersion = $matches[1]
    }
    else {
        Write-Host "Could not determine plugin version. Using 'dev' as fallback." -ForegroundColor Yellow
        $PluginVersion = "dev"
    }
}

# Create release directory
$releaseDir = Join-Path $rootDir "releases\v$PluginVersion"
if (-not (Test-Path $releaseDir)) {
    New-Item -ItemType Directory -Path $releaseDir -Force | Out-Null
}

# Copy DLL to plugin directory
$pluginDir = Join-Path $releaseDir "BepInEx\plugins\Essentials"
if (-not (Test-Path $pluginDir)) {
    New-Item -ItemType Directory -Path $pluginDir -Force | Out-Null
}
Copy-Item -Path (Join-Path $rootDir "bin\Release\netstandard2.1\Essentials.dll") -Destination $pluginDir

# Create BepInEx config directory
$configDir = Join-Path $releaseDir "BepInEx\config"
if (-not (Test-Path $configDir)) {
    New-Item -ItemType Directory -Path $configDir -Force | Out-Null
}

# Copy config file
$configFilePath = Join-Path $rootDir "src\Configs\org.ccarrmcmahon.plugins.repo.essentials.cfg"
Copy-Item -Path $configFilePath -Destination $configDir

# Copy support files to top level directory
# Copy manifest
$manifestPath = Join-Path $rootDir "manifest.json"
Copy-Item -Path $manifestPath -Destination $releaseDir

# Copy README.md (required for Thunderstore)
$readmePath = Join-Path $rootDir "README.md"
if (Test-Path $readmePath) {
    Copy-Item -Path $readmePath -Destination $releaseDir
}
else {
    Write-Host "WARNING: README.md not found! This is required for Thunderstore." -ForegroundColor Red
}

# Copy CHANGELOG.md
$changelogPath = Join-Path $rootDir "CHANGELOG.md"
if (Test-Path $changelogPath) {
    Copy-Item -Path $changelogPath -Destination $releaseDir
    Write-Host "Added CHANGELOG.md to release package." -ForegroundColor Cyan
}
else {
    Write-Host "WARNING: CHANGELOG.md not found! Consider adding a changelog to track version changes." -ForegroundColor Yellow
}

# Copy icon.png (required for Thunderstore)
$iconPath = Join-Path $rootDir "icon.png"
if (Test-Path $iconPath) {
    Copy-Item -Path $iconPath -Destination $releaseDir
}
else {
    Write-Host "WARNING: icon.png not found! This is required for Thunderstore." -ForegroundColor Red
    Write-Host "Please create a 256x256 PNG icon file named 'icon.png' in the root directory." -ForegroundColor Red
}

# Zip the contents of the release directory (both BepInEx folder and top-level files)
$zipPath = Join-Path $releaseDir "Essentials_v$PluginVersion.zip"
$zipItems = Get-ChildItem -Path $releaseDir -Exclude "*.zip"
Compress-Archive -Path $zipItems -DestinationPath $zipPath -Force

# Display information about the build
Write-Host "Release package created at: $zipPath" -ForegroundColor Green

# Get current values for display
$gameInfoPath = Join-Path $rootDir "src\Configs\GameInfo.cs"
$gameInfoContent = Get-Content $gameInfoPath -Raw

if ($gameInfoContent -match 'public const string VERSION = "([^"]+)";') {
    $currentGameVersion = $matches[1]
}

if ($gameInfoContent -match 'public const string BUILD_ID = "([^"]+)";') {
    $currentBuildID = $matches[1]
}

Write-Host "Plugin version: $PluginVersion" -ForegroundColor Yellow
Write-Host "Game version: $currentGameVersion" -ForegroundColor Yellow
Write-Host "Build ID: $currentBuildID" -ForegroundColor Yellow
