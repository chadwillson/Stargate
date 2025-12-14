@echo off
echo Starting Stargate API and UI...
echo.

REM Create logs directory if it doesn't exist
if not exist "E:\Stargate\logs" mkdir "E:\Stargate\logs"

REM Get current date for log filename
for /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set mydate=%%c%%a%%b)

REM Log startup
echo [%date% %time%] Starting Stargate services... > "E:\Stargate\logs\startup-%mydate%.log" 2>&1

REM Start API in new window (Serilog already logs everything to file)
echo [%date% %time%] Launching API... >> "E:\Stargate\logs\startup-%mydate%.log" 2>&1
start "Stargate API" cmd /k "cd /d E:\Stargate\Stargate.Api && dotnet run"

REM Wait 3 seconds for API to start
timeout /t 3 /nobreak > nul

REM Start UI in new window (Angular CLI output to console, runtime errors via UI logging service)
echo [%date% %time%] Launching UI... >> "E:\Stargate\logs\startup-%mydate%.log" 2>&1
start "Stargate UI" cmd /k "cd /d E:\Stargate\Stargate.UI && npm start"

echo.
echo Both services are starting...
echo API will be available at: http://localhost:5031
echo UI will be available at: http://localhost:4200
echo.
echo All logs are in: E:\Stargate\logs\
echo.
echo   startup-%mydate%.log - Batch file startup events
echo   stargate-%mydate%.log - API logs (Serilog) + UI runtime errors
echo.
echo Runtime errors from the browser are automatically logged via UI Logging Service.
echo Build/compilation errors will be visible in the console windows.
echo.
echo Close the windows or press Ctrl+C to stop each service.
pause
