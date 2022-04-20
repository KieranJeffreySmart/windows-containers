# Prerequisites

In order to build and deploy you will need at least one Windows Agent. Follow this [guide](https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/v2-windows?view=azure-devops) to create a self-host agent in Azure Devops.

To perform a build there should be a version control repository such as Git, containing all binaries, a dockerfile, tokenized config files and deployment manifests. Changes to this repository will trigger the build.

# Build Pipeline

A CI pipeline is a process to build, test and publish artifacts. 
Whle it would be possible to perform these tasks on an existing pipeline that builds code and publishes binaries, the approach described here assumes the artifacts from the original application build can be served to an independent pipeline focused purely on containerization and deployment to kubernetes.

The containerization build pipeline will pull the contents of a version control repository containing the pre-built binaries. It should the build and push an image of an application container, to an image registry such as Harbor. 

This pipeline should also include steps for publishing the artifacts needed by the release pipeline, such as deployment manifests and tokenized configuration files.

When you create a build pipeline in Azure Devops, you can choose to start from an empty pipeline, select one of the existing templates which are prepopulated with the relevant steps, or copy the yaml below.

To create a pipeline from an Azure Devops empty pipeline or exiting template, follow this [guide](https://docs.microsoft.com/en-us/azure/devops/pipelines/create-first-pipeline?view=azure-devops&tabs=net%2Ctfs-2018-2%2Cbrowser) and implement the steps as described below.

The following is a template for a build pipeline that kicks off when changes are merged in the main branch of an associated version control repository. A suitable agent is selected from the pool, which will build and pushes a docker image to the registry and finally publish the release artifacts.

``` yaml
trigger:
- main

pool:
  name: TKGI
  demands:
    - agent.OS -equals Windows_NT
name: $(Build.BuildId)

variables:
  repoName: 'library/my-application'

steps:
- task: DockerInstaller@0
  inputs:
    dockerVersion: '17.09.0-ce'
- script: echo '$(Build.BuildNumber)'

- task: Docker@2
  inputs:
        containerRegistry: 'tanzu-sandbox-harbor_service_connection'
        repository: $(repoName)
        command: 'buildAndPush'
        Dockerfile: '**/Dockerfile'
        tags: '$(Build.BuildNumber)'
- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.SourcesDirectory)/manifests'
    ArtifactName: 'manifests'
    publishLocation: 'Container'
- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.SourcesDirectory)/config'
    ArtifactName: 'config'
    publishLocation: 'Container'
```
To modify the above script for a specific app you are containerizing, you will need to:
* Change the pool name to one that is available to you and has an active windows agent
* Change the repoName to be a suitable project name and image name for you desired image registry
* Change containerRegistry to be the base url for your chosen image registry
* ensure both a manifests and config folder exists in your version control repository

> Note: Setting name to Build.BuildId will set the Build.BuildNumber also, which is used to tag the image, and used in the release stages to identify the correct version of the image to deploy.