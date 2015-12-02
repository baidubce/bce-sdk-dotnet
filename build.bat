@echo off

echo == %DATE% %TIME% ==
set OutputFolderBase=output

if exist %OutputFolderBase% (
    rmdir /s /q %OutputFolderBase%
)
if not exist %OutputFolderBase% ( 
    md %OutputFolderBase%
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
xcopy /E /Y BceSdkDotNet\bin\Release\*.* %OutputFolderBase%
echo ******************* Build Release Done! ********************

@echo on
exit
