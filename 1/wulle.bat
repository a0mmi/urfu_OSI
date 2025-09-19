DEL Trash\1.txt
DEL Trash\2.bat
for /L %%i in (1000, 1, 2000) do (rmdir /s /q Trash\%%i)
rmdir /s /q Trash\3
DEL wulle.bat