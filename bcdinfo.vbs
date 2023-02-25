Const BcdLibraryString_Description = &h12000004
Const Current = "{fa926493-6f1c-4193-a414-58f0b2456d1e}"

Set objStoreClass = GetObject("winmgmts:\\.\root\wmi:BcdStore")

objStoreClass.OpenStore"", objStore
objStore.OpenObject Current, objDefault
Wscript.Echo "GUID: " & objDefault.Id
objDefault.GetElement BcdLibraryString_Description, objElement
Wscript.Echo "OS: " & objElement.String