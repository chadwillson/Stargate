# Stargate Launch Script
# Starts both API and UI in separate terminal windows

Write-Host "Starting Stargate API and UI..." -ForegroundColor Green

# Check for and stop existing Stargate.Api processes
Write-Host "Checking for existing processes..." -ForegroundColor Cyan
$apiProcesses = Get-Process | Where-Object { $_.ProcessName -eq "Stargate.Api" -or ($_.ProcessName -eq "dotnet" -and $_.MainWindowTitle -like "*Stargate.Api*") }
if ($apiProcesses) {
    Write-Host "Found $($apiProcesses.Count) existing API process(es). Stopping..." -ForegroundColor Yellow
    $apiProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
}

# Check for and stop existing node processes running on port 4200 (Angular dev server)
$nodeProcesses = Get-NetTCPConnection -LocalPort 4200 -ErrorAction SilentlyContinue | ForEach-Object { Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue }
if ($nodeProcesses) {
    Write-Host "Found existing UI process(es) on port 4200. Stopping..." -ForegroundColor Yellow
    $nodeProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
}

# Check for and stop dotnet processes running on port 5031 (API server)
$dotnetProcesses = Get-NetTCPConnection -LocalPort 5031 -ErrorAction SilentlyContinue | ForEach-Object { Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue }
if ($dotnetProcesses) {
    Write-Host "Found existing API process(es) on port 5031. Stopping..." -ForegroundColor Yellow
    $dotnetProcesses | Stop-Process -Force
    Start-Sleep -Seconds 2
}

Write-Host "All existing processes stopped." -ForegroundColor Green
Write-Host ""

# Start API in new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd 'E:\Stargate\Stargate\Stargate.Api'; Write-Host 'Starting Stargate API...' -ForegroundColor Cyan; dotnet run"

# Wait a few seconds for API to start
Start-Sleep -Seconds 3

# Start UI in new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd 'E:\Stargate\Stargate\Stargate.UI'; Write-Host 'Starting Stargate UI...' -ForegroundColor Cyan; npm start"

Write-Host "`nBoth services are starting..." -ForegroundColor Green
Write-Host "API will be available at: http://localhost:5031" -ForegroundColor Yellow
Write-Host "UI will be available at: http://localhost:4200" -ForegroundColor Yellow
Write-Host "`nPress Ctrl+C in each window to stop the services." -ForegroundColor Gray
