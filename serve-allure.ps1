# Simple HTTP server for Allure report
param(
    [int]$Port = 8080
)

Write-Host "Starting HTTP server for Allure report on port $Port..." -ForegroundColor Green

# Check if allure-report exists
if (!(Test-Path "allure-report")) {
    Write-Host "Error: allure-report directory not found!" -ForegroundColor Red
    exit 1
}

# Change to report directory
Set-Location "allure-report"

try {
    # Start HTTP server using Python
    if (Get-Command python -ErrorAction SilentlyContinue) {
        Write-Host "Using Python HTTP server..." -ForegroundColor Blue
        Write-Host "Open http://localhost:$Port in your browser" -ForegroundColor Yellow
        Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
        python -m http.server $Port
    }
    elseif (Get-Command python3 -ErrorAction SilentlyContinue) {
        Write-Host "Using Python3 HTTP server..." -ForegroundColor Blue
        Write-Host "Open http://localhost:$Port in your browser" -ForegroundColor Yellow
        Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
        python3 -m http.server $Port
    }
    else {
        Write-Host "Python not found. Trying PowerShell web server..." -ForegroundColor Yellow
        
        # Simple PowerShell HTTP listener (basic version)
        $listener = New-Object System.Net.HttpListener
        $listener.Prefixes.Add("http://localhost:$Port/")
        $listener.Start()
        
        Write-Host "PowerShell HTTP server started on http://localhost:$Port" -ForegroundColor Green
        Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Yellow
        
        try {
            while ($listener.IsListening) {
                $context = $listener.GetContext()
                $request = $context.Request
                $response = $context.Response
                
                $localPath = $request.Url.LocalPath
                if ($localPath -eq "/") {
                    $localPath = "/index.html"
                }
                
                $filePath = Join-Path (Get-Location) $localPath.TrimStart('/')
                
                if (Test-Path $filePath) {
                    $content = [System.IO.File]::ReadAllBytes($filePath)
                    $response.ContentLength64 = $content.Length
                    
                    # Set content type
                    $ext = [System.IO.Path]::GetExtension($filePath)
                    switch ($ext) {
                        ".html" { $response.ContentType = "text/html" }
                        ".css" { $response.ContentType = "text/css" }
                        ".js" { $response.ContentType = "application/javascript" }
                        ".json" { $response.ContentType = "application/json" }
                        default { $response.ContentType = "application/octet-stream" }
                    }
                    
                    $response.OutputStream.Write($content, 0, $content.Length)
                } else {
                    $response.StatusCode = 404
                    $errorBytes = [System.Text.Encoding]::UTF8.GetBytes("File not found")
                    $response.OutputStream.Write($errorBytes, 0, $errorBytes.Length)
                }
                
                $response.Close()
            }
        }
        catch {
            Write-Host "Server stopped: $($_.Exception.Message)" -ForegroundColor Yellow
        }
        finally {
            $listener.Stop()
        }
    }
}
catch {
    Write-Host "Error starting server: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    Set-Location ..
}