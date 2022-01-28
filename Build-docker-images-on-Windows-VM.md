---
date = "2022-01-25T12:00:00+02:00"
tags = ["Windows containers", "docker", "kubernetes"]
title = "Building docker images on Windows VM"
---
# Windows Containerisation Setup
## Windows 10 Docker Install
Follow the steps below in order to install Docker on Windows 10 build 19041 or later.

To install docker you must open Powershell as an administrator 
```ps
powershell Start-Process powershell -Verb runAs
```
### Install and configure WSL 2
- Enable the required Hypervisor and WSL 2 features:
	```ps
	Enable-WindowsOptionalFeature -Online -FeatureName "Microsoft-Hyper-V" -NoRestart -All
	Enable-WindowsOptionalFeature -Online -FeatureName "Containers" -NoRestart -All
	Enable-WindowsOptionalFeature -Online -FeatureName "Microsoft-Windows-Subsystem-Linux" -NoRestart -All
	Enable-WindowsOptionalFeature -Online -FeatureName "VirtualMachinePlatform" -NoRestart -All
	```
- Restart the machine
- Download and run the WSL 2 Linux kernel update, and set WSL 2 as default
	```ps
	$WebClient = New-Object System.Net.WebClient
	$WebClient.DownloadFile("https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi", "./wsl_update_x64.msi")
	msiexec /i ./wsl_update_x64.msi /quiet /qn /norestart

	wsl --set-default-version 2
	```
### Install and Configure Docker and Docker Desktop
#### Windows msi Installer
- Install docker desktop using the installer and instructions found [here](https://hub.docker.com/editions/community/docker-ce-desktop-windows)
#### Installing with Chocolatey
- Install Chocolatey
	```ps
	Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
	```
- Install Docker and docker desktop using Chocolatey by running the following command:
	```ps
	choco install docker -y
	choco install docker-desktop -y # Omit this line to avoid installing docker desktop
	```
## Windows Server 2019 Docker Install

Follow the steps below in order to install Docker on Windows Server 2019 Standard or later

To install docker you must open Powershell as an administrator 
```ps
powershell Start-Process powershell -Verb runAs
```
### Install and configure Docker
- Enable and Docker Provider and install docker package:
	```ps
	Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force
	Install-Module -Name DockerMsftProvider -Force
	Install-Package -Name docker -ProviderName DockerMsftProvider -Force
	Restart-Computer -Force
	```
## Building Windows images using Docker 

- Create a Dockerfile for a Hello World image:
	```ps
	PS > Add-Content -Path .\Dockerfile -Value "FROM mcr.microsoft.com/windows/nanoserver:1809 `nCMD echo Hello World!" -Force -Encoding UTF8
	PS > cat Dockerfile
	FROM mcr.microsoft.com/windows/nanoserver:1809
	CMD echo Hello World!
	```

- Build an image using the build command:
	```ps
	PS > docker build –tag hello-world .
	Step 1/2 : FROM mcr.microsoft.com/windows/nanoserver:1809
	1809: Pulling from windows/nanoserver
	b5c97e1d373f: Pull complete
	Digest: sha256:a608d7e96462ad9de894c98de74ac5c08c4624a40c6332d78d3a38c1939e1f62
	Status: Downloaded newer image for mcr.microsoft.com/windows/nanoserver:1809
	---> 1b0690f17ad9
	Step 2/2 : CMD echo Hello World!
	---> Running in 58d382b90226
	Removing intermediate container 58d382b90226
	---> c103589c7c6c
	Successfully built c103589c7c6c
	Successfully tagged hello-world
	```
- Check image repository using the images command
	```ps
	PS > docker images
	REPOSITORY                                  TAG                              IMAGE ID       CREATED          SIZE
	hello-world                                                                  c103589c7c6c   46 seconds ago   258MB
	mcr.microsoft.com/windows/nanoserver        1809                             1b0690f17ad9   10 days ago      258MB
	```
## Running Windows containers using Docker
Once you have the image built you can run the container. Run the following Powershell command:
```ps
PS > docker run hello-world
Hello World!
```
## References

* [How to Install Docker on Windows: Reference documentation](https://docs.docker.com/desktop/windows/install/) --- How to Install Docker on Windows: Reference documentation
* [Download Linux kernel update package: Reference documentation](https://docs.microsoft.com/en-gb/windows/wsl/install-manual#step-4---download-the-linux-kernel-update-package) --- Download Linux kernel update package: Reference documentation
* [Build and run your first Docker Windows Server container](https://www.docker.com/blog/build-your-first-docker-windows-server-container/) --- Build and run your first Docker Windows Server container: Reference documentation 
* [Linux Containers on Windows Server](https://www.b2-4ac.com/lcow-linux-containers-on-windows-server/) --- Linux Containers on Windows Server: Reference documentation 







