# Creating .Net images

In order to containerize .NET Framework applications, you need to write a Dockerfile with the commands needed to publish, install, configure and run your application in a container that can be deployed to a kubernetes cluster

This guide will take you through creating a docker file, however for more detailed information you can refer to [this guide](https://docs.docker.com/engine/reference/builder/#:~:text=A%20Dockerfile%20is%20a%20text,command%2Dline%20instructions%20in%20succession.)

The Dockerfile contains a list instructions that define base Operating System images, copies files and runs windows commands and powershell commands to install features and set configurations needed to run the application successfully in a container.

## Base images

The first instruction **FROM** specifies the base image from which you are building your image. Depending on the version of Windows and .Net version of your application you will need to use different base images. 

For Windows containers, the base image should be a Windows image that derives from Windows Server Core 2019. On top of that you need to install the components that your application requires as well as adding the Windows features needed.

When containerising applications written with ASP<span>.NET</span> and .NET Framework 4.0 and above, and hosted in IIS you can use
mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019

When containerising applications written in .NET Framework 3.5 and bellow it is recommended to use mcr.microsoft.com/windows/servercore:ltsc2019 and install .NET 3.5 as described in the section [Containerizing a .NET 3.5 application](#containerizing-a-.net-3.5-application)

### Harbor Registry and Licensed Windows Images
Windows images are licensed and so cannot be registered in local registries such as Harbor. This means layers within base images are skipped and will need to be downloaded at the moment they are deployed. 

This is also complicated by limitations to network firewalls not allowing direct access to the Microsoft Container Registry and so a proxy must be created in your harbor registry to allow access to these images. 

This can be done by creating a proxy as described in [this Harbor guide](https://goharbor.io/docs/2.1.0/administration/configure-proxy-cache/) and using the url to this proxy in your docker file as shown below

```dockerfile
FROM tanzu-sandbox-harbor.principality.net/mcr-proxy/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
```

```dockerfile
FROM tanzu-sandbox-harbor.principality.net/mcr-proxy/windows/servercore:ltsc2019
```

It is also recommended to create a common base image that can be reused across applications. To find existing base images look in [this Harbor registry]()
```dockerfile
FROM tanzu-sandbox-harbor.principality.net/library/pbsnet35:2.1
```

## Running commands
By default the Windows command prompt is used to run any commands, however the following instruction starts an instance of powershell in the container to allow you to run the powershell commands. 

```dockerfile
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'SilentlyContinue'; $ProgressPreference = 'SilentlyContinue';"]
```
Alternatively a powershell command using the powershell cli, e.g. 
```powershell 
powershell -Command echo 'Hello World!'
```

To run commands in the container you should use the **RUN** instruction followed by the command as you would normally type at a prompt.

> Note: For may of the sample commands provided here it is assumed you will be have started a powershell shell

## IIS applications

Typically you would expect a single application to run in a container and so you can add your application to IIS default site using the default application pool.

Add the following to your Dockerfile so that it can copy published files and subfolders to inetpub/wwwroot.

```dockerfile
WORKDIR /inetpub/wwwroot
COPY ./publish/. ./
```

In order to collect logs for a container you should write logs to file, in a shared folder where a background process can read these files and send logs to a centralized log manager. 

To set the destination folder for IIS logs add the following command to your Dockerfile, where "c:\var\logs" is a folder shared by your app and the log collection process.

```dockerfile
RUN c:\windows\system32\inetsrv\appcmd.exe set config -section:system.applicationHost/sites /siteDefaults.logFile.directory:"c:\var\logs" /commit:apphost
```

You can expose the default IIS site port 80 to access your apps from outside your container by adding the Expose instruction

```dockerfile
EXPOSE 80
```

Lastly, you need to specify an entry point process for the container to run when it starts.
```dockerfile
ENTRYPOINT ["c:\\ServiceMonitor.exe w3svc"]
```

## Windows Services
Windows services can be created and started inside a container in the following way.

First copy the files to an appropriate folder in the container.
```dockerfile
RUN mkdir "c:\my-windows-service\bin"
WORKDIR "c:/my-windows-service/bin"
COPY ./bin/. ./
```
Create the service using the following powershell command
```dockerfile
RUN New-Service -Name "my-windows-service" -DisplayName 'my windows service' -BinaryPathName "c:\my-windows-service\bin\my-windows-service.exe"
```
Start the service
```dockerfile
RUN Start-Service "my-windows-service"
```
As windows services run in the background this will not be enouggh to keep the container active and the container will shutdown assuming there is no application running. To resolve this you can add a powershell command with an infinite loop.
>Note: As the entry point is run as a Windowss command, you will need to use the powershell cli

```dockerfile
ENTRYPOINT ["powershell -Command 'while($true){ Start-Sleep -Seconds 10 }'"]
```

## Running commands at startup

You may want to create a script to run tasks that your application needs at startup. For example, updating configuration files for your deployment environment.

IIS example: 

```powershell
IF EXIST "C:\config\Web.config" (
  COPY /Y C:\config\Web.config C:\inetpub\wwwroot\Web.config
)
C:\ServiceMonitor.exe w3svc
```

Windows Service example:

```powershell
IF EXIST "C:\config\my-windows-service.exe.config" (
  COPY /Y C:\config\my-windows-service.exe.config C:\my-windows-service\bin
)
powershell -Command Start-Service "my-windows-service"

powershell -Command 'while($true){ Start-Sleep -Seconds 10 }'
```

> Note: You can save this script as a command file e.g run.cmd for use it as an alternative entry point 

```dockerfile
COPY run.cmd /
ENTRYPOINT ["c:\\run.cmd"]
```

## Containerizing a .NET 3.5 application

When you are building an image for .NET 3.5  application you will need to install .NET Framework 3.5, apply the latest patch of Windows update and install ngen .NET Framework 

The instructions to create an image with .NET Framework 3.5 installed as follows:

```dockerfile
RUN `
    # Install .NET Fx 3.5
    curl -fSLo microsoft-windows-netfx3.zip https://dotnetbinaries.blob.core.windows.net/dockerassets/microsoft-windows-netfx3-ltsc2019.zip `
    && tar -zxf microsoft-windows-netfx3.zip `
    && del /F /Q microsoft-windows-netfx3.zip `
    && dism /Online /Quiet /Add-Package /PackagePath:.\microsoft-windows-netfx3-ondemand-package~31bf3856ad364e35~amd64~~.cab `
    && del microsoft-windows-netfx3-ondemand-package~31bf3856ad364e35~amd64~~.cab `
    && powershell Remove-Item -Force -Recurse ${Env:TEMP}\* `
    `
    # Apply latest patch
    && curl -fSLo patch.msu http://download.windowsupdate.com/c/msdownload/update/software/secu/2022/02/windows10.0-kb5010351-x64_f7ba53f4c410299fc28400f7a21b7f616f635a7c.msu `
    && mkdir patch `
    && expand patch.msu patch -F:* `
    && del /F /Q patch.msu `
    && dism /Online /Quiet /Add-Package /PackagePath:C:\patch\windows10.0-kb5010351-x64.cab `
    && rmdir /S /Q patch `
    `
    # ngen .NET Fx
    && c:\windows\Microsoft.NET\Framework64\v2.0.50727\ngen uninstall "Microsoft.Tpm.Commands, Version=10.0.0.0, Culture=Neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=amd64" `
    && c:\windows\Microsoft.NET\Framework64\v2.0.50727\ngen update `
    && c:\windows\Microsoft.NET\Framework\v2.0.50727\ngen update
```
To host a .NET 3.5 app in IIS you will need to install the relevant features, sets error preferences and download a compatible Service Monitor

```dockerfile
RUN powershell -Command `
        $ErrorActionPreference = 'Stop'; `
        $ProgressPreference = 'SilentlyContinue'; `
        Add-WindowsFeature Web-Server; `
        Add-WindowsFeature Web-Asp-Net; `
        Remove-Item -Recurse C:\inetpub\wwwroot\*; `
        Invoke-WebRequest -Uri https://dotnetbinaries.blob.core.windows.net/servicemonitor/2.0.1.10/ServiceMonitor.exe -OutFile C:\ServiceMonitor.exe
```

You will also need to set the application pool to use .NET 2.0 and install the required features
```dockerfile
RUN c:\windows\System32\inetsrv\appcmd set apppool /apppool.name:DefaultAppPool /managedRuntimeVersion:v2.0
```
## Containerising a WCF application
When you are containerizing WCF applications you need to add the following windows features to your Dockerfile

NET-WCF-HTTP-Activation45 (.NET 4.0)
NET-HTTP-Activation (.NET 3.5)
Web-WebSockets

```dockerfile
RUN Add-WindowsFeature NET-WCF-HTTP-Activation45
RUN Add-WindowsFeature Web-WebSockets
```

## Containerising ASP<span>.Net</span> app with 32 bits apps

When you are containerizing 32 bits applications you need to enable 32 bit workers on the application pool using the following instruction

```dockerfile
RUN c:\windows\system32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /enable32BitAppOnWin64:true
```

## Application pool and Pipeline Mode Classic
 Your application may require to set the deafult application pool to 2.0 runtime version and/or set the pipeline mode to classic on the application pool

```dockerfile
RUN c:\Windows\System32\inetsrv\appcmd.exe set apppool /apppool.name:DefaultAppPool /managedPipelineMode:Classic
```

An example .NET 4.5 application with environment specific configuration

```dockerfile
# escape=`
FROM harbor.principality.net/mcr-proxy/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
# start powershell
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'SilentlyContinue'; $ProgressPreference = 'SilentlyContinue';"]
# Configuring logs
RUN c:\windows\system32\inetsrv\appcmd.exe set config -section:system.applicationHost/sites /siteDefaults.logFile.directory:"c:\var\logs" /commit:apphost
# Exposing port 80
EXPOSE 80
# Copy webservice files and folders
WORKDIR /inetpub/wwwroot
COPY ./publish/. ./
# Create entry point for run.cmd to update web.config file
COPY run.cmd /
ENTRYPOINT ["c:\\run.cmd"]
```