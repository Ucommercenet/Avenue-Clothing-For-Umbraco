@echo off
rem Run from Tools folder
rem Usage
rem Deploy ..\..\src\uCommerce.RazorStore C:\Programming\Sites\UmbracoAcc
IF /I [%1]==[] GOTO :MISSING_PARAM
IF /I [%2]==[] GOTO :MISSING_PARAM

REM First remove quotes around the string
REM Remove trailing \ if it exists
set path1=%1
set path1=%path1:"=%
set lastChar=%path1:~-1%
IF /I %lastChar%==\  set path1=%path1:~0,-1%
w3
REM First remove quotes around the string
REM Remove trailing \ if it exists
set path2=%2
set path2=%path2:"=%
set lastChar=%path2:~-1%
IF /I %lastChar%==\  set path2=%path2:~0,-1%

echo %path1%
echo %path2%

robocopy "%path1%\css" "%path2%\css" /s
robocopy "%path1%\fonts" "%path2%\fonts" /s
robocopy "%path1%\scripts" "%path2%\scripts" /s
robocopy "%path1%\Views" "%path2%\Views" /s
robocopy "%path1%\img" "%path2%\img" /s
robocopy "%path1%\js" "%path2%\js" /s
robocopy "%path1%\bin" "%path2%\bin" uCommerce.RazorStore.dll
REM robocopy "%path1%\bin" "%path2%\bin" ServiceStack*.*

GOTO :DONE

:MISSING_PARAM
	echo Missing source or target directory.
	echo Usage DEPLOY source_directory target_directory
	echo Example DEPLOY .\MyWebsite \\SERVER\c$\Inetpub\wwwroot\MyWebSite

:DONE