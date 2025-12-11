@echo off
echo Starting Stargate API and UI...
echo.

REM Start API in new window
start "Stargate API" cmd /k "cd /d E:\Stargate\Stargate.Api && dotnet run"

REM Wait 3 seconds for API to start
timeout /t 3 /nobreak > nul

REM Start UI in new window
start "Stargate UI" cmd /k "cd /d E:\Stargate\Stargate.UI && npm start"

echo.
echo Both services are starting...
echo API will be available at: http://localhost:5031
echo UI will be available at: http://localhost:4200
echo.
echo Close the windows or press Ctrl+C to stop each service.
pause
