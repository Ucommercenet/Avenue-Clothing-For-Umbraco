@echo off
IF /I [%1]==[] GOTO :HELP
IF /I [%1]==[-?] GOTO :HELP
IF /I [%2]==[-q] GOTO :SILENT_EXEC

echo CLEANPACKAGE will delete files from target %1. Continue? [y/n]
set /p Continue=

IF NOT [%Continue%]==[y] GOTO :TERMINATED

:SILENT_EXEC
rem Clean root. uCommerce doesn't anything here
del %1\*.* /q

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

rem Delete files stuff created by the package installer or files not needed for clean install
del %1\config\xsltExtensions.config
del %1\umbraco\config\create\UI.xml
del %1\usercontrols\*.* /q

rem Clean debug symbols and xml docs
del %1\bin\*.xml /q

rem Delete debug symbols
del %1\bin\*.pdb /q
del %1\bin\*.pdb /q

rem Remove test assemblies
del %1\bin\*.test.dll /q

rem Remove diagnostics assemblies
del %1\bin\HibernatingRhinos.*

rem Remove InstallPage.aspx
del %1\umbraco\ucommerce\install\installpage.aspx
del %1\umbraco\ucommerce\install\uninstall.aspx
del %1\*.nl-nl.resx /s

rem Remove Mac finder files
del %1\*.ds_store* /a:h /s
del %1\*._.ds_store* /a:h /s

rem Remove Hg merge files
del %1\*.orig /s /q

GOTO :DONE

:HELP
	echo Missing target directory to clean.
	echo Usage CLEANPACKAGE target_directory
	echo Example CLEANPACKAGE c:\tmp\MyPackage
	echo For silent execution using parameter -q
	
	goto :DONE

:TERMINATED
	echo Cancelled CLEANPACKAGE
	
:DONE