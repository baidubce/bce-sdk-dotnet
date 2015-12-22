@echo off

echo == %DATE% %TIME% ==
set OutputFolderBase=output
set OutputFolderTempBase=outputtemp
set SampleFolder=%OutputFolderTempBase%\samples
set ThirdPartyFolder=%OutputFolderTempBase%\thirdparty
set Version=1.0.1

if exist %OutputFolderTempBase% (
    rmdir /s /q %OutputFolderTempBase%
)
if not exist %OutputFolderTempBase% ( 
    md %OutputFolderTempBase%
)

if exist %OutputFolderBase% (
    rmdir /s /q %OutputFolderBase%
)
if not exist %OutputFolderBase% ( 
    md %OutputFolderBase%
)

if not exist %SampleFolder% ( 
    md %SampleFolder%
)

if not exist %ThirdPartyFolder% ( 
    md %ThirdPartyFolder%
)

echo == %DATE% %TIME% ==
set PATH=D:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE;%PATH%
::set PATH=C:\Program Files\Microsoft Visual Studio 12.0\Common7\IDE;%PATH%
::set PATH=C:\WINDOWS\Microsoft.NET\Framework\v3.5;%PATH%
set PATH=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319;%PATH%

:: compile release
echo == %DATE% %TIME% ==
echo Compile Sources for Release...
msbuild BceSdkDotNet.sln /t:Clean;Rebuild /m:4 /p:Configuration=Release > x86.release.compile.log
xcopy /E /Y BceSdkDotNet\bin\Release\BceSdkDotNet.dll %OutputFolderTempBase%
xcopy /E /Y BceSdkDotNet\bin\Release\BceSdkDotNet.XML %OutputFolderTempBase%
xcopy /E /Y BceSdkDotNet\bin\Release\Newtonsoft.Json.* %ThirdPartyFolder%
xcopy /E /Y BceSdkDotNet\bin\Release\log4net.* %ThirdPartyFolder%
echo ******************* Build Release Done! ********************

:: zip package
xcopy samples\*.* %SampleFolder%
copy CHANGELOG.md %OutputFolderTempBase%
copy README.md %OutputFolderTempBase%
7za a %OutputFolderTempBase%\bce-dotnet-sdk-%Version%.zip .\%OutputFolderTempBase%\*
copy %OutputFolderTempBase%\bce-dotnet-sdk-%Version%.zip %OutputFolderBase%\bce-dotnet-sdk-%Version%.zip
pause
@echo on
exit
