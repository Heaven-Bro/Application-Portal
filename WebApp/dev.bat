@echo off
echo Starting Development Environment...
echo.

start "Tailwind Watch" cmd /k "cd /d %~dp0 && tailwindcss.exe -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --watch"

timeout /t 2 /nobreak > nul

start "Blazor App" cmd /k "cd /d %~dp0 && dotnet watch run"

echo.
echo Both processes started!
echo Close both terminal windows when done.
