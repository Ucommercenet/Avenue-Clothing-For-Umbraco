@echo off
rem Run from Tools folder
rem Usage
rem echo "Usage CSR <machine to deploy to>"
rem robocopy ..\src\csr\Vertica.DSG.CSR.WebSite \\%1\inetpub\wwwroot\CSR /e /purge /xf *.cs /xf *.??proj /xf *.vspscc /xd obj /xd _Resharper*
IF /I [%1]==[] GOTO :MISSING_PARAM
IF /I [%2]==[] GOTO :MISSING_PARAM

REM Use TEMP for target
SET workingDir=%TEMP%\uCommerceTmp\8e0acd5c-f842-49db-933d-cc9e61fcff51

REM Clean working directory
echo Cleaning working directory %workingDir%
rmdir "%workingDir%" /s /q

REM deploy
REM Deploy.cmd %1 %2
call Deploy.cmd "%1" "%workingDir%"

REM Overwrite the debug UCommerce.Umbraco.dll assembly copied in by Deploy process
robocopy "%1\..\UCommerce.Umbraco\obj\release" "%workingPath%\bin" *.dll

REM Pauses the script for a while
REM PING 1.1.1.1 -n 1 -w 60000 >NUL

REM Copy in latest database scripts
robocopy ..\..\database "%workingDir%"\umbraco\ucommerce\install uCommerceDB.???.sql

REM Copy Images to Umbraco image folder
REM robocopy ..\..\src\UCommerceWeb\ucommerce\images\ui "%workingDir%"\umbraco\images\umbraco *.png *.gif
REM robocopy ..\..\src\UCommerceWeb\ucommerce\images\tray "%workingDir%"\umbraco\images\tray *png *.gif

REM Remove Umbraco specific files, we don't want to overwrite existing Umbraco stuff
REM call CleanPackage.cmd %2 -q
call CleanPackage.cmd "%workingDir%" -q

REM Rename uninstall assembly
move %workingDir%\bin\UCommerce.Uninstaller.dll %workingDir%\bin\UCommerce.Uninstaller.dll.tmp

REM Grab the current working directory as we're changing the directory in the next step
SET currentDir=%CD%

REM Move to configuration dir as REN command doesn't work with paths
cd %workingDir%\ucommerce\configuration\

REM Rename configs to *.config.default in preparation for backup of existing files when rolling new configs during install
ren *.config *.config.default

REM Change to the pipeline dir to prepare for rename
cd %workingDir%\ucommerce\pipelines\

REM Rename pipelines to *.config.default in preparation for backup of existing files when rolling new pipelines during install
ren *.config *.config.default

REM Move back to the original dir
cd %currentDir%

REM Copy in XML definitions for package gen
copy "%1\..\UCommerce.Umbraco\Installer\*.xml" "%workingDir%\umbraco\ucommerce\install\"

REM Copy in default configs to be merged with Umbraco upon install
copy "%1\..\UCommerce.Umbraco\Installer\*.config" "%workingDir%\umbraco\ucommerce\install\"

mkdir "%workingDir%\umbraco\ucommerce\install\LanguageText\"
copy "%1\..\UCommerce.Umbraco\Installer\LanguageText\*.xml" "%workingDir%\umbraco\ucommerce\install\LanguageText\"

REM Generate package.xml
REM Generates manifest for included files and vendor package information
UCommerce.PackageGen %workingDir%

REM Clear destination file
del %2

REM Create the compressed package (standard zip archieve)
REM -mx9 means highest compression level
..\7zip\7z.exe a -r -tZip -mx9 "%2" "%workingDir%"

GOTO :DONE

:MISSING_PARAM
	echo Missing source or target directory.
	echo Usage GENERATEPACKAGE source_directory package_file
	echo Example GENERATEPACKAGE .\MyWebsite c:\uCommerce-1.0.0.0.zip

:DONE
	echo Package %2 created OK
	echo Temp location is %workingDir%