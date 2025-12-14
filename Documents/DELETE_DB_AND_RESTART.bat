@echo off
cd /d E:\Stargate\Stargate.Api
echo Stopping any running processes...
taskkill /F /IM Stargate.Api.exe 2>nul
timeout /t 2 /nobreak >nul

echo Deleting old database files...
del /F /Q stargate.db 2>nul
del /F /Q stargate.db-shm 2>nul
del /F /Q stargate.db-wal 2>nul

echo Building the API...
dotnet build

echo.
echo Database deleted and API rebuilt!
echo Now run: dotnet run
echo.
pause
