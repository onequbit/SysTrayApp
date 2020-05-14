if ($args.Length -ne 2)
{
    "invalid number of parameters"
    exit
}

$link = $args[1]

$startup = "$Env:USERPROFILE\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup"
if ((Test-Path $StartUp) -eq $false)
{
    "error: can't find startup path"
    exit
}

$shortcutFile = "$startup\$link.lnk"
if ((Test-Path $shortcutFile) -eq $true)
{
    "shortcut already exists"
    exit
}

$app = $args[0]
if ((Test-Path $app) -eq $false)
{
    "invalid file specified"
    exit
}
$path = (Get-Location).Path
$target = "$path\" + $app
if ((Test-Path $target) -eq $false)
{
    "error: target not found"
    exit
}

$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$startup\$link.lnk")
$Shortcut.TargetPath = "$target"
$Shortcut.Arguments = ""
$Shortcut.WorkingDirectory = "$path"
$Shortcut.Save()

