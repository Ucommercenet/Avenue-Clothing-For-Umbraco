@echo off

cd ..\..\

rem Delete the exisiting packages folder and any contents
del package\Avenue_Clothing_Umbraco8_6.0.0.19122\61fc5d84-9cc2-4d36-93a8-bfe0d076b219 /Q
del package\Avenue_Clothing_Umbraco8_6.0.0.19122 /Q
del package\_ToPackage /Q
del package /Q
rd package\Avenue_Clothing_Umbraco8_6.0.0.19122\61fc5d84-9cc2-4d36-93a8-bfe0d076b219 /Q
rd package\Avenue_Clothing_Umbraco8_6.0.0.19122 /Q
rd package /Q

rem Create the package directory
rem md package\Avenue_Clothing_Umbraco8_6.0.0.19122\61fc5d84-9cc2-4d36-93a8-bfe0d076b219

rem Copy over the store files which will be included in the XML
robocopy src\uCommerce.RazorStore package\_ToPackage\files *.css *.master /s /FFT /Z /XA:H /W:5

rem Copy over the store files which will be zipped
robocopy src\uCommerce.RazorStore package\_ToPackage\files uCommerceApiRegistration.cs FacetedQueryStringExtensions.cs *.cshtml *.js *.png *.jpg *.jpeg *.gif *.eot *.svq *.ttf *.woff *.woff2 *.otf /s /FFT /Z /XA:H /W:5 /xf *packages.config
robocopy src\UCommerce.RazorStore\bin package\_ToPackage\files\bin uCommerce.RazorStore.dll /FFT /Z /XA:H /W:5


rem Copy over the installer files
robocopy src\UCommerce.RazorStore.Installer package\_ToPackage\files *.ascx *.js *.png *.jpg *.jpeg *.gif *.config /s /FFT /Z /XA:H /W:5 /xf *packages.config
robocopy src\UCommerce.RazorStore.Installer\bin package\_ToPackage\files\bin  UCommerce.RazorStore.Installer.dll XmlConfigMerge.dll /FFT /Z /XA:H /W:5
robocopy src\UCommerce.RazorStore.Installer\XmlStubs package\_ToPackage *.xml /FFT /Z /XA:H /W:5

rem Package the various files
tools\deploy\PackageGen.exe -name="package\Avenue_Clothing_Umbraco8_6.0.0.19122.zip" -guid="61fc5d84-9cc2-4d36-93a8-bfe0d076b219" -path="package\_ToPackage"

GOTO :DONE

:DONE
cd tools\deploy