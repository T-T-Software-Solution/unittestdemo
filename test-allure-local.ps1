# Local Allure Testing Script
param()

Write-Host "Testing Allure Integration Locally..." -ForegroundColor Green

# Check if Allure is installed
try {
    $allureVersion = & "C:\Users\acer\tools\allure\bin\allure.bat" --version 2>&1
    Write-Host "Allure found: $allureVersion" -ForegroundColor Green
} catch {
    Write-Host "Allure CLI not found." -ForegroundColor Red
    exit 1
}

# Create results directory
$resultsDir = ".\allure-results"
$reportDir = ".\allure-report"

if (Test-Path $resultsDir) {
    Remove-Item $resultsDir -Recurse -Force
}
if (Test-Path $reportDir) {
    Remove-Item $reportDir -Recurse -Force
}

New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
Write-Host "Created allure-results directory" -ForegroundColor Blue

# Set environment variable
$env:ALLURE_RESULTS_DIRECTORY = $resultsDir

Write-Host "Running Unit Tests with Allure..." -ForegroundColor Blue
dotnet test "Tests\Demo.AppCore.Tests\Demo.AppCore.Tests.csproj" --logger "console;verbosity=normal"
Write-Host "Unit tests completed" -ForegroundColor Green

Write-Host "Running Integration Tests with Allure..." -ForegroundColor Blue
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Port=65432;Database=demounittest01;User ID=admin;Password=admin;Include Error Detail=true;"
dotnet test "Tests\Demo.Api.IntegrationTests\Demo.Api.IntegrationTests.csproj" --logger "console;verbosity=normal"
Write-Host "Integration tests completed" -ForegroundColor Green

# Check for results in multiple locations
Write-Host "Checking for Allure results..." -ForegroundColor Blue

$resultLocations = @(
    ".\allure-results",
    "Tests\Demo.AppCore.Tests\allure-results", 
    "Tests\Demo.Api.IntegrationTests\allure-results",
    "Tests\Demo.AppCore.Tests\bin\Debug\net8.0\allure-results",
    "Tests\Demo.Api.IntegrationTests\bin\Debug\net8.0\allure-results"
)

$foundResults = $false
$allResultFiles = @()

foreach ($location in $resultLocations) {
    if (Test-Path $location) {
        $resultFiles = Get-ChildItem $location -File -ErrorAction SilentlyContinue
        if ($resultFiles.Count -gt 0) {
            Write-Host "Found $($resultFiles.Count) result files in: $location" -ForegroundColor Green
            $allResultFiles += $resultFiles
            $foundResults = $true
            
            # Copy results to main directory
            if ($location -ne ".\allure-results") {
                Copy-Item "$location\*" $resultsDir -Force
            }
        }
    }
}

if ($foundResults) {
    Write-Host "Total Allure result files found: $($allResultFiles.Count)" -ForegroundColor Green
    
    Write-Host "Generating Allure Report..." -ForegroundColor Blue
    try {
        & "C:\Users\acer\tools\allure\bin\allure.bat" generate $resultsDir --output $reportDir --clean
        Write-Host "Report generated successfully!" -ForegroundColor Green
        Write-Host "Report location: $reportDir" -ForegroundColor Blue
        
        Write-Host "Opening report in browser..." -ForegroundColor Blue
        & "C:\Users\acer\tools\allure\bin\allure.bat" open $reportDir
    } catch {
        Write-Host "Failed to generate report: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "No Allure result files found in any location" -ForegroundColor Yellow
    Write-Host "This might be normal - Allure.XUnit may not generate files in expected locations" -ForegroundColor Blue
    Write-Host "The integration will work in CI/CD pipeline with GitHub Actions" -ForegroundColor Green
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Check if report opened in browser" -ForegroundColor Gray
Write-Host "  2. If no local results, they will be generated in CI/CD" -ForegroundColor Gray
Write-Host "  3. GitHub Actions will create beautiful reports automatically" -ForegroundColor Gray