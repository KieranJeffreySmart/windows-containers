#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat 

FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
RUN c:\windows\system32\inetsrv\appcmd.exe set config -section:system.applicationHost/sites /siteDefaults.logFile.directory:"c:\var\log\iis" /commit:apphost

WORKDIR /inetpub/wwwroot
COPY ./publish/. .
