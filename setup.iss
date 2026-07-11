[Setup]
AppName=MicMute
AppVersion=1.0.0
DefaultDirName={commonpf32}\MicMute
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

[InstallDelete]
Type: files; Name: "{app}\assets\sounds\muted.wav"
Type: files; Name: "{app}\assets\sounds\unmuted.wav"

[UninstallDelete]
; Remove as chaves do Registry criadas pelo app
Type: files; Name: "{app}\assets\sounds\muted.mp3"
Type: files; Name: "{app}\assets\sounds\unmuted.mp3"

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
Source: "d:\Developer\sandbox\MicMute\assets\sounds\muted.mp3"; DestDir: "{app}\assets\sounds"; Flags: ignoreversion skipifsourcedoesntexist
Source: "d:\Developer\sandbox\MicMute\assets\sounds\unmuted.mp3"; DestDir: "{app}\assets\sounds"; Flags: ignoreversion skipifsourcedoesntexist


[Icons]
Name: "{group}\MicMute"; Filename: "{app}\MicMute.exe"
Name: "{commondesktop}\MicMute"; Filename: "{app}\MicMute.exe"

[Run]
Filename: "{app}\MicMute.exe"; Description: "Iniciar o MicMute"; Flags: nowait postinstall skipifsilent

[Code]
// ---------------------------------------------------------------------------
// Verifica se o .NET Framework 4.8 está instalado
// ---------------------------------------------------------------------------
function IsDotNetDetector(): Boolean;
var
  key: string;
  release: Cardinal;
begin
  Result := False;
  key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
  if RegQueryDWordValue(HKLM, key, 'Release', release) then
  begin
    if release >= 528040 then // 528040 corresponds to .NET Framework 4.8
      Result := True;
  end;
end;

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
begin
  Result := True;
  if not IsDotNetDetector() then
  begin
    if MsgBox('Este programa requer o Microsoft .NET Framework 4.8.' + #13#10 +
              'Deseja fazer o download do .NET Framework 4.8 agora?', mbConfirmation, MB_YESNO) = idYes then
    begin
      ShellExec('open', 'https://dotnet.microsoft.com/en-us/download/dotnet-framework/net48', '', '', SW_SHOWNORMAL, ewNoWait, ErrorCode);
    end;
    Result := False;
  end;
end;

// ---------------------------------------------------------------------------
// Renomeia o desinstalador de "unins000" para "uninstall"
// O Inno Setup cria sempre o par unins000.exe + unins000.dat;
// ambos precisam ser renomeados juntos para o desinstalador continuar
// funcionando (o .exe procura o .dat com o mesmo nome base ao lado de si).
// Por fim, atualiza as entradas UninstallString no Registry para refletir
// o novo nome, de modo que o Painel de Controle / Configuracoes ainda
// consiga acionar a desinstalacao corretamente.
// ---------------------------------------------------------------------------
procedure CurStepChanged(CurStep: TSetupStep);
var
  OldExe, NewExe, OldDat, NewDat: string;
  UninstallKey: string;
begin
  if CurStep = ssPostInstall then
  begin
    OldExe := ExpandConstant('{app}\unins000.exe');
    NewExe := ExpandConstant('{app}\uninstall.exe');
    OldDat := ExpandConstant('{app}\unins000.dat');
    NewDat := ExpandConstant('{app}\uninstall.dat');

    if FileExists(OldExe) then
    begin
      // Renomeia o par .exe + .dat (mesmo diretorio, sem copiar)
      RenameFile(OldExe, NewExe);
      if FileExists(OldDat) then
        RenameFile(OldDat, NewDat);

      // Atualiza as entradas do Registry que apontam para o desinstalador.
      // O Inno Setup cria essa chave automaticamente durante a instalacao.
      UninstallKey := 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\MicMute_is1';
      RegWriteStringValue(HKLM, UninstallKey, 'UninstallString',
        '"' + NewExe + '"');
      RegWriteStringValue(HKLM, UninstallKey, 'QuietUninstallString',
        '"' + NewExe + '" /SILENT');
    end;
  end;
end;

// ---------------------------------------------------------------------------
// Limpeza dos dados do usuário durante a desinstalação
// Executado DEPOIS que todos os arquivos do app já foram removidos (usPostUninstall),
// evitando qualquer risco de deletar a pasta antes do término do processo principal.
// ---------------------------------------------------------------------------
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  AppDataPath: string;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    // Monta o caminho exato: %LocalAppData%\MicMute
    // O Inno Setup resolve {localappdata} para o AppData\Local do usuário atual,
    // nunca para uma pasta do sistema — portanto é seguro.
    AppDataPath := ExpandConstant('{localappdata}\MicMute');

    // Só age se a pasta realmente existir
    if DirExists(AppDataPath) then
    begin
      if MsgBox(
        'Deseja apagar os dados salvos do MicMute?' + #13#10 +
        '(configurações e histórico em AppData\Local\MicMute)' + #13#10#13#10 +
        'Escolha Não se pretende reinstalar o aplicativo e quer manter suas configurações.',
        mbConfirmation, MB_YESNO) = IDYES then
      begin
        // DelTree(Path, DeleteDir, DeleteFiles, DeleteSubDirs)
        // Apaga arquivos, subpastas e a pasta raiz — equivalente a "rd /s /q"
        // mas gerenciado pelo próprio Inno Setup, sem chamar shell externo.
        DelTree(AppDataPath, True, True, True);
      end;
    end;

    // Remove a chave do Registry criada pelo próprio app (SOFTWARE\MicMute)
    // RegDeleteKeyIncludingSubkeys garante remoção completa mesmo com subchaves
    RegDeleteKeyIncludingSubkeys(HKCU, 'SOFTWARE\MicMute');
  end;
end;
