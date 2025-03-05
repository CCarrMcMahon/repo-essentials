param (
    [Parameter(Mandatory=$false)]
    [string]$PluginVersion = "",
    
    [Parameter(Mandatory=$false)]
    [string]$GameVersion = "",
    
    [Parameter(Mandatory=$false)]
    [string]$BuildID = ""
)

$ErrorActionPreference = "Stop"

# Project root directory
$rootDir = $PSScriptRoot

# Track if versions were provided as parameters
$pluginVersionProvided = -not [string]::IsNullOrEmpty($PluginVersion)
$gameVersionProvided = -not [string]::IsNullOrEmpty($GameVersion)
$buildIDProvided = -not [string]::IsNullOrEmpty($BuildID)

# Ask for versions only if user wants to update them
if (-not $pluginVersionProvided) {
    $updatePlugin = Read-Host "Do you want to update plugin version? (y/n)"
    if ($updatePlugin -eq "y") {
        $PluginVersion = Read-Host "Enter plugin version (e.g. 0.1.0)"
        $pluginVersionProvided = $true
    }
}

if (-not $gameVersionProvided) {
    $updateGame = Read-Host "Do you want to update game version? (y/n)"
    if ($updateGame -eq "y") {
        $GameVersion = Read-Host "Enter game version (e.g. 0.1.2)"
        $gameVersionProvided = $true
    }
}

if (-not $buildIDProvided) {
    $updateBuildID = Read-Host "Do you want to update build ID? (y/n)"
    if ($updateBuildID -eq "y") {
        $BuildID = Read-Host "Enter game build ID (e.g. 17560228)"
        $buildIDProvided = $true
    }
}

# Only update files if there are changes to make
if ($pluginVersionProvided -or $gameVersionProvided -or $buildIDProvided) {
    Write-Host "Updating version information in files..." -ForegroundColor Cyan

    # Update GameInfo.cs if needed
    if ($gameVersionProvided -or $buildIDProvided) {
        $gameInfoPath = Join-Path $rootDir "src\GameInfo.cs"
        $gameInfoContent = Get-Content $gameInfoPath -Raw
        
        if ($gameVersionProvided) {
            $gameInfoContent = $gameInfoContent -replace 'public const string VERSION = "[^"]+";', "public const string VERSION = `"$GameVersion`";"
        }
        
        if ($buildIDProvided) {
            $gameInfoContent = $gameInfoContent -replace 'public const string BUILD_ID = "[^"]+";', "public const string BUILD_ID = `"$BuildID`";"
        }
        
        Set-Content -Path $gameInfoPath -Value $gameInfoContent
    }

    # Update PluginInfo.cs if needed
    if ($pluginVersionProvided) {
        $pluginInfoPath = Join-Path $rootDir "src\PluginInfo.cs"
        $pluginInfoContent = Get-Content $pluginInfoPath -Raw
        $pluginInfoContent = $pluginInfoContent -replace 'public const string VERSION = "[^"]+";', "public const string VERSION = `"$PluginVersion`";"
        Set-Content -Path $pluginInfoPath -Value $pluginInfoContent

        # Update manifest.json
        $manifestPath = Join-Path $rootDir "manifest.json"
        $manifestContent = Get-Content $manifestPath -Raw | ConvertFrom-Json
        $manifestContent.version_number = $PluginVersion
        $manifestContent | ConvertTo-Json -Depth 10 | Set-Content $manifestPath

        # Update csproj file
        $csprojPath = Join-Path $rootDir "RepoEssentials.csproj"
        $csprojContent = Get-Content $csprojPath -Raw
        $csprojContent = $csprojContent -replace '<Version>[^<]+</Version>', "<Version>$PluginVersion</Version>"
        Set-Content -Path $csprojPath -Value $csprojContent
    }
}

Write-Host "Building project..." -ForegroundColor Cyan

# Build the project
dotnet build -c Release

# For release creation, we need a plugin version
if (-not $pluginVersionProvided) {
    # Extract current version from PluginInfo.cs if not provided
    $pluginInfoPath = Join-Path $rootDir "src\PluginInfo.cs"
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

# Copy files to release directory
$pluginDir = Join-Path $releaseDir "BepInEx\plugins\Essentials"
if (-not (Test-Path $pluginDir)) {
    New-Item -ItemType Directory -Path $pluginDir -Force | Out-Null
}

# Copy DLL
Copy-Item -Path (Join-Path $rootDir "bin\Release\netstandard2.1\Essentials.dll") -Destination $pluginDir

# Copy manifest
$manifestPath = Join-Path $rootDir "manifest.json"
Copy-Item -Path $manifestPath -Destination $pluginDir

# Copy README.md (required for Thunderstore)
$readmePath = Join-Path $rootDir "README.md"
if (Test-Path $readmePath) {
    Copy-Item -Path $readmePath -Destination $pluginDir
} else {
    Write-Host "WARNING: README.md not found! This is required for Thunderstore." -ForegroundColor Red
}

# Copy icon.png (required for Thunderstore)
$iconPath = Join-Path $rootDir "icon.png"
if (Test-Path $iconPath) {
    Copy-Item -Path $iconPath -Destination $pluginDir
} else {
    Write-Host "WARNING: icon.png not found! This is required for Thunderstore." -ForegroundColor Red
    Write-Host "Please create a 256x256 PNG icon file named 'icon.png' in the root directory." -ForegroundColor Red
}

# Create a zip file
$zipPath = Join-Path $releaseDir "Essentials_v$PluginVersion.zip"
Compress-Archive -Path (Join-Path $releaseDir "BepInEx") -DestinationPath $zipPath -Force

# Display information about the build
Write-Host "Release package created at: $zipPath" -ForegroundColor Green

# Get current values for display
$gameInfoPath = Join-Path $rootDir "src\GameInfo.cs"
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
