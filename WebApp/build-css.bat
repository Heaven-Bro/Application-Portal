@echo off
echo Building Tailwind CSS...
tailwindcss.exe -i ./wwwroot/css/input.css -o ./wwwroot/css/output.css --minify
echo Done!
