[Setup]
AppName=MicMute
AppVersion=0.1.8.4-Beta
DefaultDirName={autopf}\MicMute
DisableDirPage=no
AppendDefaultDirName=no
DefaultGroupName=MicMute
OutputBaseFilename=MicMute_Setup
Compression=lzma
SolidCompression=yes
OutputDir=d:\Developer\sandbox\MicMute\bin\MicMute Installer
SetupIconFile=favicon.ico
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Dirs]
Name: "{app}"; Attribs: readonly
Name: "{app}\assets"
Name: "{app}\assets\icons"
Name: "{app}\assets\sounds"
Name: "{app}\assets\configs"
Name: "{app}\logs"
Name: "{app}\temp"

[Files]
Source: "d:\Developer\sandbox\MicMute\bin\Release\MicMute.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\bin\Release\MicMute.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\folder.ico"; DestDir: "{app}"; Flags: ignoreversion; Attribs: hidden system
Source: "d:\Developer\sandbox\MicMute\desktop.ini"; DestDir: "{app}"; Flags: ignoreversion; Attribs: hidden system
Source: "d:\Developer\sandbox\MicMute\assets\icons\micon.ico"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\icons\micmute.ico"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\icons\micon.png"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\icons\micmute.png"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\icons\github.png"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\icons\linkedin.png"; DestDir: "{app}\assets\icons"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\sounds\muted.wav"; DestDir: "{app}\assets\sounds"; Flags: ignoreversion
Source: "d:\Developer\sandbox\MicMute\assets\sounds\unmuted.wav"; DestDir: "{app}\assets\sounds"; Flags: ignoreversion


[Icons]
Name: "{group}\MicMute"; Filename: "{app}\MicMute.exe"
Name: "{commondesktop}\MicMute"; Filename: "{app}\MicMute.exe"

[Run]
Filename: "{app}\MicMute.exe"; Description: "Iniciar o MicMute"; Flags: nowait postinstall skipifsilent
