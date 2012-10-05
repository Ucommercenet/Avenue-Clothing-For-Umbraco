@echo off
rem Run from Tools folder
rem Usage
rem echo "Usage CSR <machine to deploy to>"
rem robocopy ..\src\csr\Vertica.DSG.CSR.WebSite \\%1\inetpub\wwwroot\CSR /e /purge /xf *.cs /xf *.??proj /xf *.vspscc /xd obj /xd _Resharper*
IF /I [%1]==[] GOTO :MISSING_PARAM
IF /I [%2]==[] GOTO :MISSING_PARAM

REM First remove quotes around the string
REM Remove trailing \ if it exists
set path1=%1
set path1=%path1:"=%
set lastChar=%path1:~-1%
IF /I %lastChar%==\  set path1=%path1:~0,-1%

REM First remove quotes around the string
REM Remove trailing \ if it exists
set path2=%2
set path2=%path2:"=%
set lastChar=%path2:~-1%
IF /I %lastChar%==\  set path2=%path2:~0,-1%

echo %path1%
echo %path2%

robocopy "%path1%\App_Code" "%path2%\App_Code" /s
robocopy "%path1%\macroScripts" "%path2%\macroScripts" 
robocopy "%path1%\css" "%path2%\css"
robocopy "%path1%\img" "%path2%\img"
robocopy "%path1%\js" "%path2%\js"
robocopy "%path1%\masterpages" "%path2%\masterpages"
robocopy "%path1%\bin" "%path2%\bin" uCommerce.RazorStore.dll
robocopy "%path1%\bin" "%path2%\bin" ServiceStack*.*
robocopy "%path1%\umbraco\ucommerce\install" "%path2%\umbraco\ucommerce\install" DemoStoreInstaller.ascx

GOTO :DONE

:MISSING_PARAM
	echo Missing source or target directory.
	echo Usage DEPLOY source_directory target_directory
	echo Example DEPLOY .\MyWebsite \\SERVER\c$\Inetpub\wwwroot\MyWebSite

:DONE