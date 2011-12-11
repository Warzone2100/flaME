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

!define PROGRAMNAME "FlaME"

!define SRCCODEDIR "${PROGRAMNAME}"
!define EXESOURCELOC "${SRCCODEDIR}\bin\Debug"
!define INTERFACEDIR "${EXESOURCELOC}\interface"

!define INTERFACESUBDIR "interface"

!define INSTALLERDATA "Windows Installer"

!define PROGRAMVMAJOR "1"
!define PROGRAMVMINOR "25"
!define FLAMEVSHORT "${PROGRAMVMAJOR}.${PROGRAMVMINOR}"
!define FLAMEVFULL "${FLAMEVSHORT}.0.0"
!define FLAMEFULLNAME "${PROGRAMNAME} ${FLAMEVSHORT}"
!define UNDERSCOREFULLNAME "${PROGRAMNAME}_${PROGRAMVMAJOR}_${PROGRAMVMINOR}"

;---- Is better if all of these are unique
!define FMAP_REG "${PROGRAMNAME} Map File (${FLAMEFULLNAME})"
!define FME_REG "${PROGRAMNAME} Old Map File (${FLAMEFULLNAME})"
!define WZ_REG "Warzone 2100 Archive (${FLAMEFULLNAME})"
!define LND_REG "EditWorld Map File (${FLAMEFULLNAME})"
!define GAM_REG "Warzone 2100 Game File (${FLAMEFULLNAME})"
	
;--------------------------------
;General
	CRCCheck on	 ;make sure this isn't corrupted
	SetCompressor /SOLID	lzma

	;Name and file
	Name "${FLAMEFULLNAME}"
	OutFile "Install ${FLAMEFULLNAME}.exe"

	;Default installation folder
	InstallDir "$PROGRAMFILES\${FLAMEFULLNAME}"

	;Request application privileges for Windows Vista
	RequestExecutionLevel admin

;--------------------------------
;Versioninfo

VIProductVersion "${FLAMEVFULL}"
VIAddVersionKey "CompanyName"		""
VIAddVersionKey "FileDescription"	"${FLAMEFULLNAME} Installer"
VIAddVersionKey "FileVersion"		"${FLAMEVFULL}"
VIAddVersionKey "InternalName"		"${FLAMEFULLNAME}"
VIAddVersionKey "LegalCopyright"	"${PROGRAMNAME} Copyright © 2010-2011 Flail13"
VIAddVersionKey "OriginalFilename"	"${FLAMEFULLNAME}.exe"
VIAddVersionKey "ProductName"		"${FLAMEFULLNAME}"
VIAddVersionKey "ProductVersion"	"${FLAMEVFULL}"
VIAddVersionKey "Installer"			"Installer script Copyright © 2010-2011 Milo Christiansen"

;--------------------------------
;Variables

	Var StartMenuFolder
	
;--------------------------------
;Interface Settings

	!define MUI_HEADERIMAGE
	!define MUI_HEADERIMAGE_BITMAP "${INSTALLERDATA}\Header.bmp"
	!define MUI_HEADERIMAGE_RIGHT
	
	!define MUI_WELCOMEFINISHPAGE_BITMAP "${INSTALLERDATA}\Welcome.bmp"
	!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${INSTALLERDATA}\Welcome.bmp"

	!define MUI_ICON "${INSTALLERDATA}\flaME.ico"
	!define MUI_UNICON "${INSTALLERDATA}\Uninstall.ico"

	!define MUI_ABORTWARNING

	; These indented statements modify settings for MUI_PAGE_FINISH
	!define MUI_FINISHPAGE_NOAUTOCLOSE
	!define MUI_UNFINISHPAGE_NOAUTOCLOSE
	!define MUI_FINISHPAGE_RUN
	!define MUI_FINISHPAGE_RUN_NOTCHECKED
	!define MUI_FINISHPAGE_RUN_TEXT "Launch ${PROGRAMNAME}"
	!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchLink"
	;!define MUI_FINISHPAGE_SHOWREADME_NOTCHECKED
	;!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\Readme.txt"
	
	;Start Menu Folder Page Configuration
	!define MUI_STARTMENUPAGE_DEFAULTFOLDER "${FLAMEFULLNAME}"
	!define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
	!define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\${FLAMEFULLNAME}" 
	!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"

;--------------------------------
;Pages

	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_COMPONENTS
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_STARTMENU "${UNDERSCOREFULLNAME}" $StartMenuFolder
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
Section "Install ${PROGRAMNAME}" Main

	SectionIn RO
	
	SetOutPath "$INSTDIR"
	File "${EXESOURCELOC}\OpenTK.GLControl.dll"
	File "${EXESOURCELOC}\OpenTK.dll"
	File "${EXESOURCELOC}\OpenTK.dll.config"
	File "${EXESOURCELOC}\ICSharpCode.SharpZipLib.dll"
	File "${EXESOURCELOC}\Matrix3D.dll"
	File "${EXESOURCELOC}\notile.png"
	File "${EXESOURCELOC}\overflow.png"
	File "${EXESOURCELOC}\FlaME.exe"
	File "${EXESOURCELOC}\flaME.ico"
	File "${INSTALLERDATA}\map.ico"

	SetOutPath "$INSTDIR\${INTERFACESUBDIR}"
	File "${INTERFACEDIR}\displayautotexture.png"
	File "${INTERFACEDIR}\drawtileorientation.png"
	File "${INTERFACEDIR}\gateways.png"
	File "${INTERFACEDIR}\objectsselect.png"
	File "${INTERFACEDIR}\save.png"
	File "${INTERFACEDIR}\selection.png"
	File "${INTERFACEDIR}\selectioncopy.png"
	File "${INTERFACEDIR}\selectionflipx.png"
	File "${INTERFACEDIR}\selectionpaste.png"
	File "${INTERFACEDIR}\selectionpasteoptions.png"
	File "${INTERFACEDIR}\selectionrotateanticlockwise.png"
	File "${INTERFACEDIR}\selectionrotateclockwise.png"
	
	;Startmenu shortcuts
	!insertmacro MUI_STARTMENU_WRITE_BEGIN "${UNDERSCOREFULLNAME}"
		CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\${FLAMEFULLNAME}.lnk" "$INSTDIR\FlaME.exe"
		CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall ${FLAMEFULLNAME}.lnk" "$INSTDIR\Uninstall.exe"
	!insertmacro MUI_STARTMENU_WRITE_END
	
	;Store installation folder
	WriteRegStr HKCU "Software\${FLAMEFULLNAME}" "" $INSTDIR
	
	;Register with add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}" "DisplayName" "${FLAMEFULLNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}" "UninstallString" '"$INSTDIR\Uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}" "NoModify" "1"
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}" "NoRepair" "1"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}" "Publisher" "Flail13"
	
	;Create uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
SectionEnd

SubSection "File Associations"

Section "FlaME .fmap" RegFMAP
	${registerExtension} "$INSTDIR\FlaME.exe" ".fmap" "${FMAP_REG}" "$INSTDIR\map.ico"
SectionEnd

Section "FlaME .fme" RegFME
	${registerExtension} "$INSTDIR\FlaME.exe" ".fme" "${FME_REG}" "$INSTDIR\map.ico"
SectionEnd

Section "Editworld .lnd" RegLND
	${registerExtension} "$INSTDIR\FlaME.exe" ".lnd" "${LND_REG}" "$INSTDIR\map.ico"
SectionEnd

Section "Warzone .wz" RegWZ
	${registerExtension} "$INSTDIR\FlaME.exe" ".wz" "${WZ_REG}" "$INSTDIR\map.ico"
SectionEnd

Section "Warzone .gam" RegGam
	${registerExtension} "$INSTDIR\FlaME.exe" ".gam" "${GAM_REG}" "$INSTDIR\map.ico"
SectionEnd

SubSectionEnd

Section "Desktop Shortcut" ShortcutDT
	SetOutPath "$INSTDIR"	
	CreateShortCut "$DESKTOP\${FLAMEFULLNAME}.lnk" "$INSTDIR\FlaME.exe"
SectionEnd

;--------------------------------
;Installer Functions

Function LaunchLink
	Exec '$INSTDIR\FlaME.exe'
FunctionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_Main ${LANG_ENGLISH} "Install ${PROGRAMNAME}."
LangString DESC_RegFMAP ${LANG_ENGLISH} "Open .fmap files when double clicked."
LangString DESC_RegFME ${LANG_ENGLISH} "Open .fme files when double clicked."
LangString DESC_RegLND ${LANG_ENGLISH} "Open .lnd files when double clicked."
LangString DESC_RegWZ ${LANG_ENGLISH} "Open .wz files when double clicked."
LangString DESC_RegGam ${LANG_ENGLISH} "Open .gam files when double clicked."

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${Main} $(DESC_Main)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegFMAP} $(DESC_RegFMAP)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegFME} $(DESC_RegFME)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegLND} $(DESC_RegLND)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegWZ} $(DESC_RegWZ)
	!insertmacro MUI_DESCRIPTION_TEXT ${RegGam} $(DESC_RegGam)
!insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "un.Uninstall ${PROGRAMNAME}" unMain

	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FLAMEFULLNAME}"
	
	Delete "$INSTDIR\Uninstall.exe"
	Delete "$INSTDIR\map.ico"
	Delete "$INSTDIR\flaME.ico"
	Delete "$INSTDIR\FlaME.exe"
	Delete "$INSTDIR\ICSharpCode.SharpZipLib.dll"
	Delete "$INSTDIR\notile.png"
	Delete "$INSTDIR\overflow.png"
	Delete "$INSTDIR\OpenTK.dll"
	Delete "$INSTDIR\OpenTK.dll.config"
	Delete "$INSTDIR\OpenTK.GLControl.dll"
	Delete "$INSTDIR\Matrix3D.dll"

	Delete "$INSTDIR\${INTERFACESUBDIR}\displayautotexture.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\drawtileorientation.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\gateways.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\objectsselect.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\save.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selection.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectioncopy.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectionflipx.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectionpaste.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectionpasteoptions.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectionrotateanticlockwise.png"
	Delete "$INSTDIR\${INTERFACESUBDIR}\selectionrotateclockwise.png"

	RMDir "$INSTDIR\${INTERFACESUBDIR}"
	RMDir "$INSTDIR"

	!insertmacro MUI_STARTMENU_GETFOLDER "${UNDERSCOREFULLNAME}" $StartMenuFolder
	
	Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall ${FLAMEFULLNAME}.lnk"
	Delete "$SMPROGRAMS\$StartMenuFolder\${FLAMEFULLNAME}.lnk"
	RMDir "$SMPROGRAMS\$StartMenuFolder"
	
	DeleteRegKey /ifempty HKCU "Software\${FLAMEFULLNAME}"
	
	; remove the desktop shortcut icon
	Delete "$DESKTOP\${FLAMEFULLNAME}.lnk"
	
	${unregisterExtension} ".fmap" "${FMAP_REG}"
	${unregisterExtension} ".fme" "${FME_REG}"
	${unregisterExtension} ".wz" "${WZ_REG}"
	${unregisterExtension} ".lnd" "${LND_REG}"
	${unregisterExtension} ".gam" "${GAM_REG}"
SectionEnd

;--------------------------------
;Installer Section descriptions

LangString DESC_unMain ${LANG_ENGLISH} "Uninstall ${PROGRAMNAME}."

!insertmacro MUI_UNFUNCTION_DESCRIPTION_BEGIN
	!insertmacro MUI_DESCRIPTION_TEXT ${unMain} $(DESC_unMain)
!insertmacro MUI_UNFUNCTION_DESCRIPTION_END
