@echo off
if "%~1"=="" exit /b 1
set "patt=%~1"
set "exts=%PATHEXT:;= %"

pushd "%CD%" 2>nul
for %%E in (%exts%) do for %%F in (%patt%%%E) do if exist "%%~fF" echo %CD%\%%F
for %%F in (%patt%) do if exist "%%~fF" echo %CD%\%%F
popd

set "p=%PATH%"
:lp
if "%p%"=="" goto :e
for /f "tokens=1* delims=;" %%A in ("%p%") do (set "d=%%~A" & set "p=%%B")
set "d=!d:"=!"
if not exist "!d!" goto :lp
pushd "!d!" 2>nul || goto :lp
for %%E in (%exts%) do for %%F in (%patt%%%E) do if exist "%%~fF" echo !d!\%%F
for %%F in (%patt%) do if exist "%%~fF" echo !d!\%%F
popd
goto :lp
:e
exit /b
