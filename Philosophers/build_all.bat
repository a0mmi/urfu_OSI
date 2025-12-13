@echo off
set CONFIG=Release

echo ============================
echo Building all philosophers
echo ============================

for %%D in (
    philosophers_os_deadlock
    philosophers_os1
    philosophers_os2
    philosophers_os3
    philosophers_os4
) do (
    echo.
    echo --- Building %%D ---
    dotnet build %%D -c %CONFIG%
    if errorlevel 1 exit /b 1
)

echo.
echo All projects built successfully!
pause
