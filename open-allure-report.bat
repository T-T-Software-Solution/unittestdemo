@echo off
echo Opening Allure Report...

cd allure-report

REM Start HTTP server in background
echo Starting HTTP server on port 8080...
start "Allure HTTP Server" cmd /c "python -m http.server 8080"

REM Wait a moment for server to start
timeout /t 3 /nobreak >nul

REM Open browser
echo Opening browser to http://localhost:8080
start http://localhost:8080

echo.
echo Allure report is now running at http://localhost:8080
echo Close the HTTP server window to stop serving the report
echo.
pause