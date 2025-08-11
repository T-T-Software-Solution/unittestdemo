# Allure CLI Installation Script for Windows
param()

Write-Host "Installing Allure CLI for Windows..." -ForegroundColor Green

# Check if Java is installed
try {
    $javaVersion = java -version 2>&1
    if ($javaVersion -like "*version*") {
        Write-Host "Java found: $($javaVersion[0])" -ForegroundColor Green
    }
} catch {
    Write-Host "Java not found. Please install Java 8+ first:" -ForegroundColor Red
    Write-Host "Download from: https://adoptopenjdk.net/" -ForegroundColor Yellow
    exit 1
}

# Create tools directory
$toolsDir = "$env:USERPROFILE\tools"
$allureDir = "$toolsDir\allure"

if (!(Test-Path $toolsDir)) {
    New-Item -ItemType Directory -Path $toolsDir -Force | Out-Null
    Write-Host "Created tools directory: $toolsDir" -ForegroundColor Blue
}

# Download Allure
$allureVersion = "2.24.1"
$allureUrl = "https://github.com/allure-framework/allure2/releases/download/$allureVersion/allure-$allureVersion.zip"
$zipPath = "$toolsDir\allure-$allureVersion.zip"

Write-Host "Downloading Allure $allureVersion..." -ForegroundColor Blue
try {
    Invoke-WebRequest -Uri $allureUrl -OutFile $zipPath -UseBasicParsing
    Write-Host "Downloaded successfully" -ForegroundColor Green
} catch {
    Write-Host "Failed to download Allure: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Extract Allure
Write-Host "Extracting Allure..." -ForegroundColor Blue
try {
    if (Test-Path $allureDir) {
        Remove-Item $allureDir -Recurse -Force
    }
    
    Expand-Archive -Path $zipPath -DestinationPath $toolsDir -Force
    Rename-Item "$toolsDir\allure-$allureVersion" $allureDir
    
    Write-Host "Extracted to: $allureDir" -ForegroundColor Green
} catch {
    Write-Host "Failed to extract: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Add to PATH
$allurebinPath = "$allureDir\bin"
$currentPath = [Environment]::GetEnvironmentVariable("Path", [EnvironmentVariableTarget]::User)

if ($currentPath -notlike "*$allurebinPath*") {
    Write-Host "Adding Allure to PATH..." -ForegroundColor Blue
    $newPath = "$currentPath;$allurebinPath"
    [Environment]::SetEnvironmentVariable("Path", $newPath, [EnvironmentVariableTarget]::User)
    $env:Path = "$env:Path;$allurebinPath"
    Write-Host "Added to PATH" -ForegroundColor Green
} else {
    Write-Host "Allure already in PATH" -ForegroundColor Yellow
}

# Test installation
Write-Host "Testing Allure installation..." -ForegroundColor Blue
try {
    $allureVersionOutput = & "$allurebinPath\allure.bat" --version 2>&1
    Write-Host "Allure installed successfully!" -ForegroundColor Green
    Write-Host "Version: $allureVersionOutput" -ForegroundColor Green
} catch {
    Write-Host "Allure test failed. You may need to restart your terminal." -ForegroundColor Red
}

# Cleanup
Remove-Item $zipPath -Force
Write-Host "Cleaned up download files" -ForegroundColor Blue

Write-Host ""
Write-Host "Installation complete!" -ForegroundColor Green
Write-Host "You may need to restart your terminal for PATH changes to take effect." -ForegroundColor Yellow
Write-Host "Test with: allure --version" -ForegroundColor Blue