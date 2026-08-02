#define AppName "مترجم الشاشة"
#define AppEnglishName "Screen Translator"
#define AppVersion "1.0.0"
#define AppPublisher "Screen Translator"
#define AppExeName "ScreenTranslate.exe"

[Setup]
AppId={{8F38D732-186C-4A82-904A-798B801E7633}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppEnglishName} {#AppVersion}
AppPublisher={#AppPublisher}
VersionInfoVersion=1.0.0.0
VersionInfoCompany={#AppPublisher}
VersionInfoDescription={#AppEnglishName} Setup
VersionInfoProductName={#AppEnglishName}
VersionInfoProductVersion={#AppVersion}
DefaultDirName={autopf64}\Screen Translator
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0.10240
OutputDir=..\releases
OutputBaseFilename=ScreenTranslate-1.0.0-Setup
SetupIconFile=..\ScreenTranslate\Assets\app.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName} {#AppVersion}
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no
LicenseFile=..\PRIVACY.md
InfoBeforeFile=..\README.md

[Languages]
Name: "arabic"; MessagesFile: "compiler:Languages\Arabic.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"; WorkingDir: "{app}"
Name: "{group}\إزالة تثبيت {#AppName}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\{#AppExeName}"; Description: "تشغيل {#AppName}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNet48Installed: Boolean;
var
  Release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM64, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', Release) and (Release >= 528040);
end;

function InitializeSetup: Boolean;
begin
  Result := IsDotNet48Installed;
  if not Result then
    MsgBox('يتطلب مترجم الشاشة Microsoft .NET Framework 4.8 أو أحدث.', mbError, MB_OK);
end;
