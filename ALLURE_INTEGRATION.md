# Allure Reporting Integration

## Overview
This project now includes comprehensive Allure reporting integration for both unit and integration tests, providing rich visual test reports with detailed execution information.

## What's Implemented

### 1. **NuGet Packages Added**
- `Allure.XUnit 2.12.1` - Allure adapter for xUnit tests
- Added to both test projects:
  - `Tests/Demo.AppCore.Tests/Demo.AppCore.Tests.csproj`
  - `Tests/Demo.Api.IntegrationTests/Demo.Api.IntegrationTests.csproj`

### 2. **Test Annotations**
All existing tests now include rich Allure metadata:

#### Unit Tests (Demo.AppCore.Tests)
- **Student Model Tests**: `[AllureFeature("Student Model")]`
  - Student Creation, Properties, and Exam Results stories
- **Grade Calculation Tests**: `[AllureFeature("Grade Calculation")]`
  - Weighted Grade Calculation, Edge Cases, Score Validation stories
  - Letter Grade Assignment, Exam Weight Handling stories

#### Integration Tests (Demo.Api.IntegrationTests)
- **Students API Tests**: `[AllureFeature("Students API")]`
  - Get Students, Create Student, Update Student, Delete Student stories
- **Grade Calculation API Tests**: `[AllureFeature("Grade Calculation API")]`

### 3. **Severity Levels**
Tests are categorized by importance:
- `SeverityLevel.critical` - Core functionality (CRUD operations, grade calculations)
- `SeverityLevel.normal` - Edge cases and validation tests

### 4. **GitHub Actions Integration**
Updated CI pipeline (`/.github/workflows/ci.yml`) includes:
- Allure CLI installation (v2.24.1)
- Allure results collection from both test projects
- HTML report generation
- Artifact upload for test results and Allure reports
- GitHub Pages deployment for easy report access

### 5. **Configuration Files**
- `allure.json` - Global and per-project Allure configuration
- `test.runsettings` - Test execution settings with Allure environment variables

## Features

### Rich Test Reports
- **Test Organization**: Tests grouped by Features and Stories
- **Execution Details**: Step-by-step test execution with timing
- **Historical Trends**: Track test success/failure over time
- **Attachments**: Support for screenshots, logs, and other artifacts
- **Parallel Integration**: Works alongside existing coverage reports

### GitHub Integration
- Automatic report generation on CI runs
- GitHub Pages deployment for easy access
- Report artifacts available for download
- Integration with existing coverage reporting

## How to Use

### Local Development

#### Option 1: Quick Start (Recommended)
```bash
# Use the provided batch file to run tests and open report
open-allure-report.bat
```

#### Option 2: Manual Steps
```bash
# Install Allure CLI (one-time setup)
powershell -ExecutionPolicy Bypass -File install-allure.ps1

# Run tests with Allure reporting
set ALLURE_RESULTS_DIRECTORY=allure-results
dotnet test "Tests\Demo.AppCore.Tests\Demo.AppCore.Tests.csproj"
dotnet test "Tests\Demo.Api.IntegrationTests\Demo.Api.IntegrationTests.csproj"

# Generate and serve report
allure generate allure-results --output allure-report --clean
allure open allure-report
```

#### Option 3: Using Built-in Scripts
```bash
# Use comprehensive local testing script
powershell -ExecutionPolicy Bypass -File test-allure-local.ps1
```

**Note**: Allure reports require an HTTP server to display properly. The provided scripts automatically start a local server at `http://localhost:8080`.

### CI/CD Pipeline
- Reports automatically generated on main branch pushes
- Available as GitHub Actions artifacts
- Deployed to GitHub Pages at `https://[username].github.io/[repo]/allure-report`

### Adding New Tests
When creating new tests, include Allure attributes:

```csharp
[Fact]
[AllureFeature("Your Feature Name")]
[AllureStory("Your Story Name")]
[AllureSeverity(SeverityLevel.critical)]
[AllureDescription("Detailed description of what the test verifies")]
public async Task YourTestMethod()
{
    // Test implementation with AAA pattern
}
```

## Verification
✅ Allure packages successfully installed and integrated
✅ All existing tests annotated with Allure metadata
✅ GitHub Actions workflow updated for report generation
✅ Local test execution shows "Allure reporter enabled"
✅ Build and compilation successful
✅ **Local report generation and viewing confirmed working**
✅ **37 tests successfully executed with rich Allure metadata**
✅ **HTTP server integration for proper report display**

## Test Results Summary (Latest Run)
- **Total Tests**: 37 (27 Unit + 10 Integration)
- **Status**: All Passed ✅
- **Duration**: ~15 seconds
- **Report**: Available at `http://localhost:8080` via provided scripts

## Benefits
- **Enhanced Visibility**: Beautiful, professional test reports
- **Better Organization**: Tests grouped by features and user stories
- **Historical Tracking**: Trend analysis and test stability metrics
- **Team Collaboration**: Easy-to-share visual reports
- **CI/CD Integration**: Automated report generation and deployment
- **Parallel Reporting**: Complements existing coverage reports without conflicts

The integration maintains compatibility with existing xUnit/Moq/Coverlet setup while adding rich reporting capabilities.

## Available Scripts
- `install-allure.ps1` - One-time Allure CLI installation
- `test-allure-local.ps1` - Comprehensive test runner with report generation
- `serve-allure.ps1` - HTTP server for serving existing reports
- `open-allure-report.bat` - Quick start for testing and viewing reports
- `allure-test.bat` - Alternative test runner script

## Troubleshooting
- **Report shows "loading"**: Ensure you're accessing via HTTP server (`http://localhost:8080`), not file:// protocol
- **No test results**: Verify `ALLURE_RESULTS_DIRECTORY` environment variable is set
- **Server won't start**: Check if port 8080 is already in use, or modify scripts to use different port