@echo off
sn -k keyfile.snk
csc /target:winexe /win32icon:onequbit.ico /keyfile:keyfile.snk systrayApp.cs
del keyfile.snk
