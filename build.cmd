@echo off
call clean.cmd
call define.cmd
sn -k keyfile.snk
csc %compileoptions% %buildfiles%
if %errorlevel% neq 0 exit /b %errorlevel%
del keyfile.snk
call signit.cmd
copy /y %outfile% ..\ToolBin
