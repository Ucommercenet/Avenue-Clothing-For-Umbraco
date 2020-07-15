@echo off

cd ..\..\

SET version=7.2.0.20195

rem Delete the exisiting packages folder and any contents
del package\Avenue_Clothing_Umbraco8_%version%\61fc5d84-9cc2-4d36-93a8-bfe0d076b219 /Q
del package\Avenue_Clothing_Umbraco8_%version% /Q
del package\_ToPackage /Q
del package /Q
rd package\Avenue_Clothing_Umbraco8_%version%\61fc5d84-9cc2-4d36-93a8-bfe0d076b219 /Q
rd package\Avenue_Clothing_Umbraco8_%version% /Q
rd package /Q

rem Create the package directory
md package\Avenue_Clothing_Umbraco8_%version%\61fc5d84-9cc2-4d36-93a8-bfe0d076b219

rem Copy over the store files which will be included in the XML
robocopy src\AvenueClothing package\_ToPackage\files *.css *.master /s /FFT /Z /XA:H /W:5

rem Copy over the store files which will be zipped
robocopy src\AvenueClothing package\_ToPackage\files uCommerceApiRegistration.cs FacetedQueryStringExtensions.cs *.cshtml *.js *.png *.jpg *.jpeg *.gif *.eot *.svq *.ttf *.woff *.woff2 *.otf /s /FFT /Z /XA:H /W:5 /xf *packages.config
robocopy src\AvenueClothing\bin package\_ToPackage\files\bin AvenueClothing.dll /FFT /Z /XA:H /W:5

rem Copy AvenueProductsIndexDefinition.config into a separate app
robocopy src\AvenueClothing\umbraco\Ucommerce\Apps package\_ToPackage\files\umbraco\Ucommerce\Apps /E

rem Copy over the installer files
robocopy src\AvenueClothing.Installer package\_ToPackage\files *.ascx *.js *.png *.jpg *.jpeg *.gif *.config /s /FFT /Z /XA:H /W:5 /xf *packages.config
robocopy src\AvenueClothing.Installer\bin package\_ToPackage\files\bin  AvenueClothing.Installer.dll XmlConfigMerge.dll /FFT /Z /XA:H /W:5
robocopy src\AvenueClothing.Installer\XmlStubs package\_ToPackage *.xml /FFT /Z /XA:H /W:5

rem Package the various files
tools\deploy\PackageGen.exe -name="package\Avenue_Clothing_Umbraco8_%version%.zip" -guid="61fc5d84-9cc2-4d36-93a8-bfe0d076b219" -path="package\_ToPackage"

GOTO :DONE

:DONE
cd tools\deploy