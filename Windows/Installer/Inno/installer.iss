; Inno Setup script (basic scaffold)
[Setup]
AppName=KSC-Sharp
AppVersion=0.1.0
DefaultDirName={pf}\KSC-Sharp
DefaultGroupName=KSC-Sharp
OutputBaseFilename=KSC-Sharp-Setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "..\..\WindowsLauncher\bin\Release\net8.0\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\KSC-Sharp"; Filename: "{app}\WindowsLauncher.exe"
Name: "{commondesktop}\KSC-Sharp"; Filename: "{app}\WindowsLauncher.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\WindowsLauncher.exe"; Description: "Launch KSC-Sharp"; Flags: nowait postinstall skipifsilent
