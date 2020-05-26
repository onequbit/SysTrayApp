# SysTrayApp

### (My first) System Tray Application
---
This tool is intended to allow you to view the status of one or more Windows Services, and provide an easy way to start or stop them.

This application requests administrator rights via UAC when it starts.

Double-click on the icon to show the status(es).

Right-click on the icon to access the menu and perform an action.

**build.cmd** - this code requires *sn.exe* (strong name tool) to be present in the user's environment PATH.

TO DO: add a config file to specify which services to monitor/control, or a custom command to execute.
