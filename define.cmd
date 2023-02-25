@echo off
set outfile=SysTrayApp.exe
set buildfiles=ExtensionMethods.cs TempFile.cs AdminProcess.cs BCDUtils.cs ServiceTools.cs SysTray*.cs
set runelevated=-win32manifest:%outfile%.manifest
set strongname=/keyfile:keyfile.snk
set appicon=-win32icon:onequbit.ico
set winexeoptions=-target:winexe %appicon% %strongname% %runelevated%
set consoleoptions=-target:exe %appicon% %strongname%
set compileoptions=-out:%outfile% %winexeoptions%
