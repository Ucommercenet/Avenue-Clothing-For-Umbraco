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

rem remove umbraco assemblies and its dependencies
del %1\bin\AjaxControlToolkit.dll
del %1\bin\businesslogic.dll
del %1\bin\cms.dll
del %1\bin\controls.dll
del %1\bin\CookComputing.XmlRpcV2.dll
del %1\bin\DotNetOpenMail.dll
del %1\bin\ICSharpCode.SharpZipLib.dll
del %1\bin\interfaces.dll
del %1\bin\Lucene.Net.dll
del %1\bin\Microsoft.Practices.*.dll
del %1\bin\Microsoft.ApplicationBlocks.*.dll
del %1\bin\MySql.Data.dll
del %1\bin\System.Data.SQLite.dll
del %1\bin\TidyNet.dll
del %1\bin\umbraco*.dll

REM Copy in Umbraco 4 specific DLLs
robocopy "%path1%\..\UCommerce.Umbraco\bin\debug" "%path2%\bin" UCommerce.Umbraco.* PackageActionsContrib.dll

robocopy "%path1%\ucommerce" "%path2%\umbraco\ucommerce" /s /xf *.cs /xf *.??proj /xf *.user /xf *.old /xf *.vspscc /xf xsltExtensions.config /xf web.config /xf uCommerce.key /xf uCommerce.key /xf *.tmp /xd obj /xd _Resharper* /xd .svn /xd _svn	

robocopy "%path1%\bin" "%path2%\bin" /s /xf *.cs /xf *.??proj /xf *.user /xf *.old /xf *.vspscc /xf xsltExtensions.config /xf web.config /xf uCommerce.key /xf uCommerce.key /xf *.tmp /xd obj /xd _Resharper* /xd .svn /xd _svn	

robocopy "%path1%\App_GlobalResources" "%path2%\App_GlobalResources" /s /xf *.cs /xf *.??proj /xf *.user /xf *.old /xf *.vspscc /xf xsltExtensions.config /xf web.config /xf uCommerce.key /xf uCommerce.key /xf *.tmp /xd obj /xd _Resharper* /xd .svn /xd _svn

mkdir "%path2%\umbraco\ucommerce\install\"

REM Copy in install user control
copy "%path1%\..\UCommerce.Umbraco\Installer\Installer.ascx" "%path2%\umbraco\ucommerce\install\Installer.ascx"

REM Copy in tray icon
robocopy "%path1%\UCommerce\Images\tray" "%path2%\umbraco\images\tray" UCommerce.gif

REM Pauses the script for a while
REM PING 1.1.1.1 -n 1 -w 60000 >NUL

GOTO :DONE

:MISSING_PARAM
	echo Missing source or target directory.
	echo Usage DEPLOY source_directory target_directory
	echo Example DEPLOY .\MyWebsite \\SERVER\c$\Inetpub\wwwroot\MyWebSite

:DONE