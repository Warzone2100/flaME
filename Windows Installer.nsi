;
;	NSIS Modern User Interface
;	flaME Installer script
;

;--------------------------------
;Include section
	
	!include "MUI2.nsh"
	!include "FileFunc.nsh"
	!include "LogicLib.nsh"
	
	!include "FileAssociation.nsh"

;--------------------------------
;Defines

!define EXESOURCELOC "FlaME\bin\VB 2010"
!define SRCCODEDIR "FlaME"
!define DATASOURCELOC "data"
!define TILESOURCELOC "data"

!define INSTALLERDATA "Windows Installer"

!define FLAMEVSHORT "1.20"
!define FLAMEVFULL "${FLAMEVSHORT}.0.0"
	
;--------------------------------
;General
	CRCCheck on	 ;make sure this isn't corrupted
	SetCompressor /SOLID	lzma

	;Name and file
	Name "FlaME ${FLAMEVSHORT}"
	OutFile "Install FlaME ${FLAMEVSHORT}.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\FlaME ${FLAMEVSHORT}"
	
	;Get installation folder from registry if available
	;InstallDirRegKey HKCU "Software\flaME\1.x" ""

	;Request application privileges for Windows Vista
	RequestExecutionLevel admin

;--------------------------------
;Versioninfo

VIProductVersion "${FLAMEVFULL}"
VIAddVersionKey "CompanyName"		""
VIAddVersionKey "FileDescription"	"FlaME ${FLAMEVSHORT} Installer"
VIAddVersionKey "FileVersion"		"${FLAMEVFULL}"
VIAddVersionKey "InternalName"		"FlaME ${FLAMEVSHORT}"
VIAddVersionKey "LegalCopyright"	"FlaME Copyright © 2010-2011 Flail13"
VIAddVersionKey "OriginalFilename"	"FlaME ${FLAMEVSHORT}.exe"
VIAddVersionKey "ProductName"		"FlaME ${FLAMEVSHORT}"
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

	!define MUI_ICON "${INSTALLERDATA}\flaME.ico"
	!define MUI_UNICON "${INSTALLERDATA}\flaME uninstall.ico"

	!define MUI_ABORTWARNING

	; These indented statements modify settings for MUI_PAGE_FINISH
	!define MUI_FINISHPAGE_NOAUTOCLOSE
	!define MUI_UNFINISHPAGE_NOAUTOCLOSE
	!define MUI_FINISHPAGE_RUN
	!define MUI_FINISHPAGE_RUN_NOTCHECKED
	!define MUI_FINISHPAGE_RUN_TEXT "Launch FlaME"
	!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
	;!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
	;!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\Readme.txt"
	
	;Start Menu Folder Page Configuration
	!define MUI_STARTMENUPAGE_DEFAULTFOLDER "FlaME"
	!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
	!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\flaME\1.x" 
	!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

;--------------------------------
;Pages

	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_STARTMENU "FlaME" $StartMenuFolder
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
Section "Install FlaME" Main

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
	File "${EXESOURCELOC}\FlaME.exe"
	File "${EXESOURCELOC}\flaME.ico"
	File "${INSTALLERDATA}\map.ico"
	
	;Startmenu shortcuts
	!insertmacro MUI_STARTMENU_WRITE_BEGIN "FlaME"
		CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\FlaME.lnk" "$INSTDIR\FlaME.exe"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall FlaME.lnk" "$INSTDIR\Uninstall.exe"
	!insertmacro MUI_STARTMENU_WRITE_END
	
	;Store installation folder
	WriteRegStr HKCU "Software\flaME\1.x" "" $INSTDIR
	
	;Register with add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "DisplayName" "FlaME ${FLAMEVSHORT}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "UninstallString" '"$INSTDIR\Uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "NoModify" "1"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "NoRepair" "1"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME" "Publisher" "Flail13"
	
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
SectionEnd

SubSection "File Associations"

Section "FlaME .fmap" RegFMAP
	${registerExtension} "$INSTDIR\FlaME.exe" ".fmap" "FlaME Map File" "$INSTDIR\map.ico"
SectionEnd

Section "FlaME .fme" RegFME
	${registerExtension} "$INSTDIR\FlaME.exe" ".fme" "FlaME Map File" "$INSTDIR\map.ico"
SectionEnd

Section "Editworld .lnd" RegLND
	${registerExtension} "$INSTDIR\FlaME.exe" ".lnd" "EditWorld Map File" "$INSTDIR\map.ico"
SectionEnd

Section "Warzone .wz" RegWZ
	${registerExtension} "$INSTDIR\FlaME.exe" ".wz" "Warzone 2100 Archive" "$INSTDIR\map.ico"
SectionEnd

SubSectionEnd

Section "Desktop Shortcut" ShortcutDT
	SetOutPath "$INSTDIR"	
	CreateShortCut "$DESKTOP\FlaME.lnk" "$INSTDIR\FlaME.exe"
SectionEnd

;--------------------------------
;Installer Functions

Function LaunchLink
	Exec '$INSTDIR\FlaME.exe'
FunctionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_Main ${LANG_ENGLISH} "Install FlaME."
LangString DESC_RegFMAP ${LANG_ENGLISH} "Open .fmap files when double clicked."
LangString DESC_RegFME ${LANG_ENGLISH} "Open .fme files when double clicked."
LangString DESC_RegLND ${LANG_ENGLISH} "Open .lnd files when double clicked."
LangString DESC_RegWZ ${LANG_ENGLISH} "Open .wz files when double clicked."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${Main} $(DESC_Main)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegFMAP} $(DESC_RegFMAP)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegFME} $(DESC_RegFME)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegLND} $(DESC_RegLND)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegWZ} $(DESC_RegWZ)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "un.Uninstall FlaME" unMain

	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\flaME"
	
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\map.ico"
	Delete "$INSTDIR\flame.ico"
	Delete "$INSTDIR\FlaME.exe"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
	Delete "$INSTDIR\notile.png"
	Delete "$INSTDIR\overflow.png"
	Delete "$INSTDIR\OpenTK.dll"
	Delete "$INSTDIR\OpenTK.GLControl.dll"
	RMDir "$INSTDIR"

	!insertmacro MUI_STARTMENU_GETFOLDER "flaME" $StartMenuFolder
	
	Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall FlaME.lnk"
	Delete "$SMPROGRAMS\$StartMenuFolder\FlaME.lnk"
	RMDir "$SMPROGRAMS\$StartMenuFolder"
	
	DeleteRegKey /ifempty HKCU "Software\flaME\1.x"
	
	; remove the desktop shortcut icon
	Delete "$DESKTOP\flaME.lnk"
	
SectionEnd

Section "un.Delete File Registration" unReg
	${unregisterExtension} ".fmap" "FlaME Map File"
	${unregisterExtension} ".fme" "FlaME Map File"
	${unregisterExtension} ".wz" "Warzone 2100 Archive"
	${unregisterExtension} ".lnd" "EditWorld Map File"
SectionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_unMain ${LANG_ENGLISH} "Uninstall FlaME."
LangString DESC_unReg ${LANG_ENGLISH} "Deregister .fmap, .fme, .lnd and .wz files."

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${unMain} $(DESC_unMain)
	!insertmacro MUI_DESCRIPTION_TEXT ${unReg} $(DESC_unReg)
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END
