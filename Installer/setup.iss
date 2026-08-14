#define MyAppName "Dokkaebi-OS"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Giorno"
#define MyAppExeName "dokkaebi-os.exe"

[Setup]
AppId={{7a88d78c-981e-49de-bb92-2cb2fbdb96fa}}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}

DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}

OutputDir=Output
OutputBaseFilename=dokkaebi-os-Setup

Compression=lzma
SolidCompression=yes

WizardStyle=modern

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Files]
Source: "..\bin\Release\net10.0\win-x64\publish\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"