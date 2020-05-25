@echo off
call clean.cmd
sn -k keyfile.snk
csc -target:winexe -win32icon:onequbit.ico -win32manifest:app.manifest /keyfile:keyfile.snk *.cs
if %errorlevel% neq 0 exit /b %errorlevel%
del keyfile.snk
