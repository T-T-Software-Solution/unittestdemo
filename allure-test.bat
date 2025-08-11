@echo off
echo Setting up Allure test environment...

REM Create allure-results directory
mkdir allure-results 2>nul

REM Set environment variable
set ALLURE_RESULTS_DIRECTORY=allure-results

REM Run tests with Allure output
echo Running unit tests...
dotnet test "Tests\Demo.AppCore.Tests\Demo.AppCore.Tests.csproj" --logger "console;verbosity=normal"

echo.
echo Checking for allure-results...
if exist "allure-results" (
    echo ✅ allure-results directory found
    dir "allure-results" /B
) else (
    echo ❌ allure-results directory not found
)

REM Check in test directory
if exist "Tests\Demo.AppCore.Tests\allure-results" (
    echo ✅ allure-results found in test directory
    dir "Tests\Demo.AppCore.Tests\allure-results" /B
) else (
    echo ❌ allure-results not found in test directory
)

echo.
echo Test complete. Check output above for Allure results.
pause