# SysTrayApp

### (My first) System Tray Application
---
This tool is intended to allow you to view the status of one or more Windows Services, and provide an easy way to start or stop them.

This application requests administrator rights via UAC when it starts.

Double-click on the icon to show the status(es).

Right-click on the icon to access the menu and perform an action.


**build.cmd** - this code requires *sn.exe* (strong name tool) to be present in the user's environment PATH. *.NET 4.8 SDK is recommended*

**debug.cmd** - sets the DEBUG flag for compilation

**options.csv** - this config file must be local to the executable. Each line must have either one or two strings surrounded by parenthesis. 

*example:*
```
"ServiceName"
```
or
```
"menu option name","command line string to run"
```
