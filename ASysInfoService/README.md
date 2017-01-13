
copy ASysInfoService.exe LibASysInfo.dll into bin directory (C:\Program Files\ASysInfo)

install service command:
$>sc create ASysInfo displayname="ASysInfo Service" binpath="C:\Program Files\ASysInfo\ASysInfoService.exe" start=auto
