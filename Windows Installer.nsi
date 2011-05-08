;
;	NSIS Modern User Interface
;	flaME Installer script
;

;--------------------------------
;Include section
	
	!include "MUI2.nsh"
	!include "FileFunc.nsh"
	!include "LogicLib.nsh"

;--------------------------------
;Defines

!define EXESOURCELOC "flaME\bin\VB 2010"
!define SRCCODEDIR "flaME"
!define DATASOURCELOC "data"
!define TILESOURCELOC "data"

;!define FRMGSOURCELOC "fRMG\bin\Debug"

!define INSTALLERDATA "Windows Installer"

!define FLAMEVSHORT "1.19"
!define FLAMEVFULL "${FLAMEVSHORT}.0.0"
	
;--------------------------------
;General
	CRCCheck on	 ;make sure this isn't corrupted
	SetCompressor /SOLID	lzma

	;Name and file
	Name "flaME ${FLAMEVSHORT}"
	OutFile "Install flaME ${FLAMEVSHORT}.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\flaME"
	
	;Get installation folder from registry if available
	InstallDirRegKey HKCU "Software\flaME\1.x" ""

	;Request application privileges for Windows Vista
	RequestExecutionLevel admin

;--------------------------------
;Versioninfo

VIProductVersion "${FLAMEVFULL}"
VIAddVersionKey "CompanyName"		""
VIAddVersionKey "FileDescription"	"flaME ${FLAMEVSHORT} Installer"
VIAddVersionKey "FileVersion"		"${FLAMEVFULL}"
VIAddVersionKey "InternalName"		"flaME ${FLAMEVSHORT}"
VIAddVersionKey "LegalCopyright"	"flaME Copyright © 2010-2011 Flail13"
VIAddVersionKey "OriginalFilename"	"flaME ${FLAMEVSHORT}.exe"
VIAddVersionKey "ProductName"		"flaME ${FLAMEVSHORT}"
VIAddVersionKey "ProductVersion"	"${FLAMEVFULL}"
VIAddVersionKey "Installer"			"Installer script Copyright © 2010-2011 Milo Christiansen"

;--------------------------------
;Variables

	Var StartMenuFolder
	
;--------------------------------
;Interface Settings

	!define MUI_HEADERIMAGE
	!define MUI_HEADERIMAGE_BITMAP "${INSTALLERDATA}\flaME header.bmp"
	!define MUI_HEADERIMAGE_RIGHT
	
	!define MUI_WELCOMEFINISHPAGE_BITMAP "${INSTALLERDATA}\flaME welcome.bmp"
	!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${INSTALLERDATA}\flaME welcome.bmp"

	!define MUI_ICON "${INSTALLERDATA}\flaME install.ico"
	!define MUI_UNICON "${INSTALLERDATA}\flaME uninstall.ico"

	!define MUI_ABORTWARNING

	; These indented statements modify settings for MUI_PAGE_FINISH
	!define MUI_FINISHPAGE_NOAUTOCLOSE
	!define MUI_UNFINISHPAGE_NOAUTOCLOSE
	!define MUI_FINISHPAGE_RUN
	!define MUI_FINISHPAGE_RUN_NOTCHECKED
	!define MUI_FINISHPAGE_RUN_TEXT "Launch flaME"
	!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
	;!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
	;!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\Readme.txt"
	
	;Start Menu Folder Page Configuration
	!define MUI_STARTMENUPAGE_DEFAULTFOLDER "flaME"
	!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
	!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\flaME\1.x" 
	!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

;--------------------------------
;Pages

	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_STARTMENU "flaME" $StartMenuFolder
	!insertmacro MUI_PAGE_INSTFILES
	!insertmacro MUI_PAGE_FINISH

	!insertmacro MUI_UNPAGE_WELCOME
	!insertmacro MUI_UNPAGE_COMPONENTS
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
	!insertmacro MUI_UNPAGE_FINISH

;--------------------------------
;Languages

	!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections
;Main install Section
Section "Install flaME" Main

	SectionIn RO

	;SetOutPath "$INSTDIR\default"
	;File "${DATASOURCELOC}\objectdata.zip"
	
	;SetOutPath "$INSTDIR\tilesets"
	;File "${TILESOURCELOC}\tilesets.zip"
	
	SetOutPath "$INSTDIR"
	File "${EXESOURCELOC}\OpenTK.GLControl.dll"
	File "${EXESOURCELOC}\OpenTK.dll"
	File "${EXESOURCELOC}\notile.png"
	File "${EXESOURCELOC}\overflow.png"
	File "${EXESOURCELOC}\ICSharpCode.SharpZipLib.dll"
	File "${EXESOURCELOC}\flaME.exe"
	File "${EXESOURCELOC}\flame.ico"

	;Add 7za
	;SetOutPath "$INSTDIR"
	;File "${INSTALLERDATA}\7za.exe"
	
	;Extract zips
	;ExecWait '"$INSTDIR\7za.exe" x "$INSTDIR\default\objectdata.zip" -o"$INSTDIR\default\"'
	;ExecWait '"$INSTDIR\7za.exe" x "$INSTDIR\tilesets\tilesets.zip" -o"$INSTDIR\tilesets\"'
	
	;Delete 7za and zips
	;Delete "$INSTDIR\7za.exe"
	;Delete "$INSTDIR\default\objectdata.zip"
	;Delete "$INSTDIR\tilesets\tilesets.zip"
	
	;Startmenu shortcuts
	!insertmacro MUI_STARTMENU_WRITE_BEGIN "flaME"
		CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\flaME.lnk" "$INSTDIR\flaME.exe"
		;CreateShortCut "$SMPROGRAMS\$StartMenuFolder\fRMG.lnk" "$INSTDIR\fRMG.exe"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall flaME.lnk" "$INSTDIR\Uninstall.exe"
	!insertmacro MUI_STARTMENU_WRITE_END
	
	;Desktop Shortcut
	SetOutPath "$INSTDIR"	
	CreateShortCut "$DESKTOP\flaME.lnk" "$INSTDIR\flaME.exe"
	
	;Store installation folder
	WriteRegStr HKCU "Software\flaME\1.x" "" $INSTDIR
	
	;Register with add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "DisplayName" "flaME ${FLAMEVSHORT}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "UninstallString" '"$INSTDIR\Uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "NoModify" "1"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "NoRepair" "1"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "Publisher" "Flail13"
	
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
SectionEnd

;;Install fRMG
;Section "Install fRMG" fRMG
;	;Store Installed - Not needed
;	;WriteRegStr HKCU "Software\flaME\1.x" "fRMG" "Y"
;	
;	;Install
;	SetOutPath "$INSTDIR"
;	File "${FRMGSOURCELOC}\fRMG.exe"
;	File "${FRMGSOURCELOC}\Pathfinder.dll"
;	
;	;Startmenu shortcuts
;	!insertmacro MUI_STARTMENU_WRITE_BEGIN "flaME"
;		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\fRMG.lnk" "$INSTDIR\fRMG.exe"
;	!insertmacro MUI_STARTMENU_WRITE_END
;	
;SectionEnd

;Register .fme, .lnd and .wz files

Section "Register flaME .fme Files" RegFME
	;Register file types
	WriteRegStr HKLM "SOFTWARE\Classes\.fme" "" "WZ2100.FME"
	
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.FME" "" "flaME Save File"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.FME\DefaultIcon" "" '"$INSTDIR\flame2.ico",0'
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.FME\shell\Open" "" "Open"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.FME\shell\Open\command" "" '"$INSTDIR\flaME.exe" "%1"'
SectionEnd

Section "Register Editworld .lnd Files" RegLND
	;Register file types
	WriteRegStr HKLM "SOFTWARE\Classes\.lnd" "" "WZ2100.LND"
	
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.LND" "" "Editworld Save File"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.LND\DefaultIcon" "" '"$INSTDIR\flame2.ico",0'
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.LND\shell\Open" "" "Open"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.LND\shell\Open\command" "" '"$INSTDIR\flaME.exe" "%1"'
SectionEnd

Section "Register Warzone .wz Files" RegWZ
	;Register file types
	WriteRegStr HKLM "SOFTWARE\Classes\.wz" "" "WZ2100.WZ"
	
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.WZ" "" "Warzone 2100 File"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.WZ\DefaultIcon" "" '"$INSTDIR\flame2.ico",0'
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.WZ\shell\Open" "" "Open"
	WriteRegStr HKLM "SOFTWARE\Classes\WZ2100.WZ\shell\Open\command" "" '"$INSTDIR\flaME.exe" "%1"'
SectionEnd

;--------------------------------
;Installer Functions

Function LaunchLink
	Exec '$INSTDIR\flaME.exe'
FunctionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_Main ${LANG_ENGLISH} "Install flaME."
;LangString DESC_fRMG ${LANG_ENGLISH} "Install fRMG."
LangString DESC_RegFME ${LANG_ENGLISH} "Open .fme files in flaME when double clicked."
LangString DESC_RegLND ${LANG_ENGLISH} "Open .lnd files in flaME when double clicked."
LangString DESC_RegWZ ${LANG_ENGLISH} "Open .wz files in flaME when double clicked."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${Main} $(DESC_Main)
	;!insertmacro MUI_DESCRIPTION_TEXT ${fRMG} $(DESC_fRMG)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegFME} $(DESC_RegFME)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegLND} $(DESC_RegLND)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegWZ} $(DESC_RegWZ)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "un.Uninstall flaME" unMain

	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME"
	
	Delete "$INSTDIR\Uninstall.exe"
	;Delete "$INSTDIR\fRMG.exe"
	Delete "$INSTDIR\flame.ico"
	Delete "$INSTDIR\flaME.exe"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
	Delete "$INSTDIR\notile.png"
	Delete "$INSTDIR\overflow.png"
	Delete "$INSTDIR\OpenTK.dll"
	Delete "$INSTDIR\OpenTK.GLControl.dll"
	RMDir "$INSTDIR"

	!insertmacro MUI_STARTMENU_GETFOLDER "flaME" $StartMenuFolder
	
	Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall flaME.lnk"
	Delete "$SMPROGRAMS\$StartMenuFolder\flaME.lnk"
	Delete "$SMPROGRAMS\$StartMenuFolder\fRMG.lnk"
	RMDir "$SMPROGRAMS\$StartMenuFolder"
	
	DeleteRegKey /ifempty HKCU "Software\flaME\1.x"
	
	; remove the desktop shortcut icon
	Delete "$DESKTOP\flaME.lnk"
	
SectionEnd

/* Now we always uninstall fRMG, so this is not needed
;Uninstall fRMG
Section "un.Uninstall fRMG" unfRMG
	;Is Installed?
	ReadRegStr $0 HKCU "Software\flaME\1.x" "fRMG"
	${If} $0 == "Y"
		;Uninstall
		Delete "$INSTDIR\fRMG.exe"
	${EndIf}
SectionEnd
*/

Section "un.Delete File Registration" unReg
	DeleteRegKey HKLM "SOFTWARE\Classes\.fme"
	DeleteRegKey HKLM "SOFTWARE\Classes\.lnd"
	DeleteRegKey HKLM "SOFTWARE\Classes\.wz"
	DeleteRegKey HKLM "SOFTWARE\Classes\WZ2100.FME"
	DeleteRegKey HKLM "SOFTWARE\Classes\WZ2100.LND"
	DeleteRegKey HKLM "SOFTWARE\Classes\WZ2100.WZ"
SectionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_unMain ${LANG_ENGLISH} "Uninstall flaME."
;LangString DESC_unfRMG ${LANG_ENGLISH} "Uninstall fRMG."
LangString DESC_unReg ${LANG_ENGLISH} "Deregister .fme, .lnd and .wz files."

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${unMain} $(DESC_unMain)
;	!insertmacro MUI_DESCRIPTION_TEXT ${unfRMG} $(DESC_unfRMG)
	!insertmacro MUI_DESCRIPTION_TEXT ${unReg} $(DESC_unReg)
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END
