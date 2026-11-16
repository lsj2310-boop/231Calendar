; 231 Calendar installer, 2026-11-16. AGPL-3.0-or-later.
Unicode true
!include "MUI2.nsh"
!include "x64.nsh"
!include "WinVer.nsh"
!include "LogicLib.nsh"
!ifndef PAYLOAD
  !define PAYLOAD "..\..\..\build\publish"
!endif
!ifndef OUTPUT
  !define OUTPUT "231Calendar_Setup_v1.0.1.exe"
!endif
Name "231 Calendar"
Caption "231 Calendar 설치"
OutFile "${OUTPUT}"
InstallDir "$LOCALAPPDATA\Programs\231 Calendar"
InstallDirRegKey HKCU "Software\231Calendar" "InstallDir"
RequestExecutionLevel user
SetCompressor /SOLID lzma
ManifestDPIAware true
VIProductVersion "1.0.1.0"
VIAddVersionKey /LANG=1042 "ProductName" "231 Calendar"
VIAddVersionKey /LANG=1042 "FileDescription" "231 Calendar 설치 프로그램"
VIAddVersionKey /LANG=1042 "FileVersion" "1.0.1"
VIAddVersionKey /LANG=1042 "LegalCopyright" "Copyright (C) 2026 Lee Sang Il (231 Calendar modifications); original code Copyright (C) 2026 Mahdi Kazemiesfahani; AGPL-3.0-or-later"
!define MUI_ICON "..\Calendar231\app.ico"
!define MUI_UNICON "..\Calendar231\app.ico"
!define MUI_ABORTWARNING
!define MUI_WELCOMEPAGE_TITLE "231 Calendar 설치"
!define MUI_WELCOMEPAGE_TEXT "Created by Lee Sangil.$\r$\nCopyright (C) 2026 Lee Sang Il (231 Calendar modifications).$\r$\n$\r$\nIncludes original code by Mahdi Kazemiesfahani, licensed under AGPL-3.0-or-later.$\r$\n$\r$\nWindows 10/11 (64-bit).$\r$\nMissing .NET 8 Desktop and WebView2 runtimes will be downloaded from Microsoft.$\r$\n$\r$\nSign in to Google Calendar after installation."
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_LICENSE "..\LICENSE"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!define MUI_FINISHPAGE_RUN "$INSTDIR\231Calendar.exe"
!define MUI_FINISHPAGE_RUN_TEXT "231 Calendar 실행"
!define MUI_FINISHPAGE_TEXT "설치가 완료되었습니다.$\r$\n$\r$\nCreated by Lee Sangil.$\r$\nCopyright (C) 2026 Lee Sang Il (231 Calendar modifications).$\r$\n$\r$\nGoogle 계정에 로그인한 뒤 상단 마우스 버튼으로 편집 / 위젯 모드를 전환하세요."
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "Korean"
Function .onInit
  ${IfNot} ${RunningX64}
    MessageBox MB_OK|MB_ICONSTOP "Windows 64비트 환경이 필요합니다."
    Quit
  ${EndIf}
  ${IfNot} ${AtLeastWin10}
    MessageBox MB_OK|MB_ICONSTOP "Windows 10 또는 11이 필요합니다."
    Quit
  ${EndIf}
  SetRegView 64
FunctionEnd
Function CheckRunning
  FindWindow $0 "" "231 Calendar"
  ${If} $0 != 0
    MessageBox MB_RETRYCANCEL|MB_ICONEXCLAMATION "실행 중인 231 Calendar를 먼저 종료해 주세요.$\r$\n알림 영역 아이콘을 마우스 오른쪽 버튼으로 클릭하여 종료할 수 있습니다." IDRETRY CheckRunningRetry
    Abort
    CheckRunningRetry:
    Call CheckRunning
  ${EndIf}
FunctionEnd
Section "231 Calendar" SEC_MAIN
  Call CheckRunning
  InitPluginsDir
  SetOutPath "$PLUGINSDIR"
  File "InstallDependencies.ps1"
  DetailPrint "필수 구성 요소 확인 및 설치 중입니다. 잠시 기다려 주세요..."
  ; Use native 64-bit PowerShell from a 32-bit NSIS installer.
  ExecWait '"$WINDIR\Sysnative\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -NonInteractive -ExecutionPolicy Bypass -File "$PLUGINSDIR\InstallDependencies.ps1"' $0
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONSTOP "필수 구성 요소를 설치하지 못했습니다.$\r$\n인터넷 연결과 Windows 관리자 권한을 확인한 뒤 다시 실행해 주세요.$\r$\n상세 기록: $LOCALAPPDATA\231Calendar\setup.log"
    Abort
  ${EndIf}
  SetOutPath "$INSTDIR"
  File /r "${PAYLOAD}\*"
  File "InstallDependencies.ps1"
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  CreateDirectory "$SMPROGRAMS\231 Calendar"
  CreateShortcut "$SMPROGRAMS\231 Calendar\231 Calendar.lnk" "$INSTDIR\231Calendar.exe"
  CreateShortcut "$SMPROGRAMS\231 Calendar\사용 안내.lnk" "$INSTDIR\사용안내.txt"
  CreateShortcut "$DESKTOP\231 Calendar.lnk" "$INSTDIR\231Calendar.exe"
  WriteRegStr HKCU "Software\231Calendar" "InstallDir" "$INSTDIR"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "DisplayName" "231 Calendar"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "DisplayVersion" "1.0.1"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "UninstallString" '$\"$INSTDIR\Uninstall.exe$\"'
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "DisplayIcon" "$INSTDIR\231Calendar.exe"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "InstallLocation" "$INSTDIR"
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "NoModify" 1
  WriteRegDWORD HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar" "NoRepair" 1
SectionEnd
Section "Uninstall"
  SetRegView 64
  FindWindow $0 "" "231 Calendar"
  ${If} $0 != 0
    MessageBox MB_OK|MB_ICONEXCLAMATION "231 Calendar를 종료한 뒤 제거해 주세요."
    Abort
  ${EndIf}
  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "231Calendar"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\231Calendar"
  DeleteRegKey HKCU "Software\231Calendar"
  Delete "$DESKTOP\231 Calendar.lnk"
  Delete "$SMPROGRAMS\231 Calendar\231 Calendar.lnk"
  Delete "$SMPROGRAMS\231 Calendar\사용 안내.lnk"
  RMDir "$SMPROGRAMS\231 Calendar"
  ; Remove only package-owned files. Keep unrelated files and the browser profile.
  !include "uninstall-files.nsh"
  Delete "$INSTDIR\InstallDependencies.ps1"
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
SectionEnd
