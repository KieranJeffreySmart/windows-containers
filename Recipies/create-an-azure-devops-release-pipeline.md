# Prerequisites

In order to release you will need at least one Windows Agent. Follow this [guide](https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/v2-windows?view=azure-devops) to create a self-host agent in Azure Devops.

You should already have created a build pipeline to trigger the release.

You will need to create a service connection for the cluster you want to deploy to. Follow this [guide](https://docs.microsoft.com/en-us/azure/devops/pipelines/library/service-endpoints?view=azure-devops&tabs=yaml) to create and manage service connections on Azure Devops.

# Release Pipeline

The release pipeline will collect the artifacts published during the build step and use the build number to deploy the latest version of an image created during build. It will also be responsible for applying configuration transforms and variable injection, and deploying config to kubernetes as a configmap

You can start from an empty pipeline and add the stages and task as described below, or you can modify and import the json below, which should create the minimum required tasks for most applications. For more on creating a release pipeline in Azure Devops follow this [guide](https://docs.microsoft.com/en-us/azure/devops/pipelines/release/define-multistage-release-process?view=azure-devops).


```json
{
	"description": null,
	"variables": {},
	"variableGroups": [],
	"environments": [
		{
			"id": 12,
			"name": "Sandbox",
			"rank": 1,
			"variables": {},
			"variableGroups": [],
			"preDeployApprovals": {
				"approvals": [
					{
						"rank": 1,
						"isAutomated": true,
						"isNotificationOn": false,
						"id": 34
					}
				],
				"approvalOptions": {
					"requiredApproverCount": null,
					"releaseCreatorCanBeApprover": false,
					"autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped": false,
					"enforceIdentityRevalidation": false,
					"timeoutInMinutes": 0,
					"executionOrder": 1
				}
			},
			"deployStep": {
				"id": 35
			},
			"postDeployApprovals": {
				"approvals": [
					{
						"rank": 1,
						"isAutomated": true,
						"isNotificationOn": false,
						"id": 36
					}
				],
				"approvalOptions": {
					"requiredApproverCount": null,
					"releaseCreatorCanBeApprover": false,
					"autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped": false,
					"enforceIdentityRevalidation": false,
					"timeoutInMinutes": 0,
					"executionOrder": 2
				}
			},
			"deployPhases": [
				{
					"deploymentInput": {
						"parallelExecution": {
							"parallelExecutionType": 0
						},
						"agentSpecification": null,
						"skipArtifactsDownload": false,
						"artifactsDownloadInput": {
							"downloadInputs": []
						},
						"demands": [
							"agent.OS -equals Windows_NT"
						],
						"enableAccessToken": false,
						"timeoutInMinutes": 0,
						"jobCancelTimeoutInMinutes": 1,
						"condition": "succeeded()",
						"overrideInputs": {}
					},
					"rank": 1,
					"phaseType": 1,
					"name": "Agent job",
					"refName": null,
					"workflowTasks": [
						{
							"environment": {},
							"taskId": "cbc316a2-586f-4def-be79-488a1f503564",
							"version": "1.*",
							"name": "Create NameSpace ",
							"refName": "",
							"enabled": true,
							"alwaysRun": false,
							"continueOnError": false,
							"timeoutInMinutes": 0,
							"retryCountOnTaskFailure": 0,
							"definitionType": "task",
							"overrideInputs": {},
							"condition": "succeeded()",
							"inputs": {
								"connectionType": "Kubernetes Service Connection",
								"kubernetesServiceEndpoint": "",
								"azureSubscriptionEndpoint": "",
								"azureResourceGroup": "",
								"kubernetesCluster": "",
								"useClusterAdmin": "false",
								"namespace": "",
								"command": "apply",
								"useConfigurationFile": "false",
								"configurationType": "configuration",
								"configuration": "",
								"inline": "",
								"arguments": "-f $(System.DefaultWorkingDirectory)/_my-application/manifests/my-application-namespace.yml",
								"secretType": "dockerRegistry",
								"secretArguments": "",
								"containerRegistryType": "Azure Container Registry",
								"dockerRegistryEndpoint": "",
								"azureSubscriptionEndpointForSecrets": "",
								"azureContainerRegistry": "",
								"secretName": "",
								"forceUpdate": "true",
								"configMapName": "",
								"forceUpdateConfigMap": "false",
								"useConfigMapFile": "false",
								"configMapFile": "",
								"configMapArguments": "",
								"versionOrLocation": "version",
								"versionSpec": "1.13.2",
								"checkLatest": "false",
								"specifyLocation": "",
								"cwd": "$(System.DefaultWorkingDirectory)",
								"outputFormat": "json"
							}
						},
						{
							"environment": {},
							"taskId": "a8515ec8-7254-4ffd-912c-86772e2b5962",
							"version": "5.*",
							"name": "Replace tokens in config files",
							"refName": "",
							"enabled": true,
							"alwaysRun": false,
							"continueOnError": false,
							"timeoutInMinutes": 0,
							"retryCountOnTaskFailure": 0,
							"definitionType": "task",
							"overrideInputs": {},
							"condition": "succeeded()",
							"inputs": {
								"rootDirectory": "$(System.DefaultWorkingDirectory)/_my-application/config",
								"targetFiles": "**/*.config",
								"encoding": "auto",
								"tokenPattern": "default",
								"tokenPrefix": "#{",
								"tokenSuffix": "}#",
								"writeBOM": "true",
								"escapeType": "auto",
								"escapeChar": "",
								"charsToEscape": "",
								"verbosity": "normal",
								"actionOnMissing": "warn",
								"keepToken": "false",
								"actionOnNoFiles": "continue",
								"enableTransforms": "false",
								"transformPrefix": "(",
								"transformSuffix": ")",
								"variableFiles": "",
								"inlineVariables": "",
								"variableSeparator": ".",
								"enableRecursion": "false",
								"useLegacyPattern": "false",
								"useLegacyEmptyFeature": "true",
								"useDefaultValue": "false",
								"emptyValue": "(empty)",
								"defaultValue": "",
								"enableTelemetry": "true"
							}
						},
						{
							"environment": {},
							"taskId": "cbc316a2-586f-4def-be79-488a1f503564",
							"version": "1.*",
							"name": "Create Web.Config ConfigMap",
							"refName": "",
							"enabled": true,
							"alwaysRun": false,
							"continueOnError": false,
							"timeoutInMinutes": 0,
							"retryCountOnTaskFailure": 0,
							"definitionType": "task",
							"overrideInputs": {},
							"condition": "succeeded()",
							"inputs": {
								"connectionType": "Kubernetes Service Connection",
								"kubernetesServiceEndpoint": "",
								"azureSubscriptionEndpoint": "",
								"azureResourceGroup": "",
								"kubernetesCluster": "",
								"useClusterAdmin": "false",
								"namespace": "my-application",
								"command": "",
								"useConfigurationFile": "false",
								"configurationType": "configuration",
								"configuration": "",
								"inline": "",
								"arguments": "",
								"secretType": "dockerRegistry",
								"secretArguments": "",
								"containerRegistryType": "Azure Container Registry",
								"dockerRegistryEndpoint": "",
								"azureSubscriptionEndpointForSecrets": "",
								"azureContainerRegistry": "",
								"secretName": "",
								"forceUpdate": "true",
								"configMapName": "my-application-webconfig-configmap",
								"forceUpdateConfigMap": "true",
								"useConfigMapFile": "false",
								"configMapFile": "",
								"configMapArguments": "--from-file=$(System.DefaultWorkingDirectory)/_my-application/config",
								"versionOrLocation": "version",
								"versionSpec": "1.13.2",
								"checkLatest": "false",
								"specifyLocation": "",
								"cwd": "$(System.DefaultWorkingDirectory)",
								"outputFormat": "json"
							}
						},
						{
							"environment": {},
							"taskId": "dee316a2-586f-4def-be79-488a1f503dfe",
							"version": "0.*",
							"name": "deploy",
							"refName": "",
							"enabled": true,
							"alwaysRun": false,
							"continueOnError": false,
							"timeoutInMinutes": 0,
							"retryCountOnTaskFailure": 0,
							"definitionType": "task",
							"overrideInputs": {},
							"condition": "succeeded()",
							"inputs": {
								"action": "deploy",
								"kubernetesServiceConnection": "",
								"namespace": "my-application",
								"strategy": "none",
								"trafficSplitMethod": "pod",
								"percentage": "0",
								"baselineAndCanaryReplicas": "1",
								"manifests": "$(System.DefaultWorkingDirectory)/_my-application/manifests/my-application-deployment.yml",
								"containers": "my-container-registry/my-application:$(Release.Artifacts._my-application.BuildNumber)\n",
								"imagePullSecrets": "",
								"renderType": "helm",
								"dockerComposeFile": "",
								"helmChart": "",
								"releaseName": "",
								"overrideFiles": "",
								"overrides": "",
								"kustomizationPath": "",
								"resourceToPatch": "file",
								"resourceFileToPatch": "",
								"kind": "",
								"name": "",
								"replicas": "",
								"mergeStrategy": "strategic",
								"arguments": "",
								"patch": "",
								"secretType": "dockerRegistry",
								"secretName": "",
								"secretArguments": "",
								"dockerRegistryEndpoint": "",
								"rolloutStatusTimeout": "0"
							}
						}
					]
				}
			],
			"environmentOptions": {
				"emailNotificationType": "OnlyOnFailure",
				"emailRecipients": "release.environment.owner;release.creator",
				"skipArtifactsDownload": false,
				"timeoutInMinutes": 0,
				"enableAccessToken": false,
				"publishDeploymentStatus": true,
				"badgeEnabled": false,
				"autoLinkWorkItems": false,
				"pullRequestDeploymentEnabled": false
			},
			"demands": [],
			"conditions": [
				{
					"name": "ReleaseStarted",
					"conditionType": 1,
					"value": ""
				}
			],
			"executionPolicy": {
				"concurrencyCount": 1,
				"queueDepthCount": 0
			},
			"schedules": [],
			"retentionPolicy": {
				"daysToKeep": 30,
				"releasesToKeep": 3,
				"retainBuild": true
			},
			"processParameters": {},
			"properties": {
				"BoardsEnvironmentType": {
					"$type": "System.String",
					"$value": "unmapped"
				},
				"LinkBoardsWorkItems": {
					"$type": "System.String",
					"$value": "False"
				}
			},
			"preDeploymentGates": {
				"id": 0,
				"gatesOptions": null,
				"gates": []
			},
			"postDeploymentGates": {
				"id": 0,
				"gatesOptions": null,
				"gates": []
			},
			"environmentTriggers": []
		}
	],
	"artifacts": [
	],
	"triggers": [],
	"releaseNameFormat": "Release-$(rev:r)",
	"tags": [],
	"properties": {
		"DefinitionCreationSource": {
			"$type": "System.String",
			"$value": "ReleaseImport"
		},
		"IntegrateBoardsWorkItems": {
			"$type": "System.String",
			"$value": "False"
		},
		"IntegrateJiraWorkItems": {
			"$type": "System.String",
			"$value": "false"
		}
	},
	"name": "my-application",
	"path": "\\",
	"projectReference": null
}
```
Modify the above json to replace my-application with the correct name for the application you are creating a release for.

When you have imported the above json you should see the Sandbox stages and 4 tasks created:
![Artifacts.JPG](/.attachments/Artifacts-aea4ead2-e20f-46ac-b5f6-43e933e78fda.JPG)
![Tasks.JPG](/.attachments/Tasks-5b84deed-0952-423a-80e4-8cad91cfa8ed.JPG)
To complete the release you will need to do the following:
1. Add a build artifact
    * Select the appropriate build pipeline that will have built and pushed the application container image, and published folder containing required configuration files and deployment manifests 
    ![NewBuildArtifact_h.JPG](/.attachments/NewBuildArtifact_h-2fb278ff-503c-4847-9d2a-46df230a3f07.JPG)
    > Note: Remember the Source alias, as you will need this to correctly configure the tasks
1. Enable the trigger to run the release on a successful build:
    ![BuildArtifactTrigger_h.JPG](/.attachments/BuildArtifactTrigger_h-73104d2e-5543-4bd7-9d70-b13e96687138.JPG)
1. Select an Agent Pool in the Agent Job task:
    * Select an Agent Pool that has a windows build agent, and ensure the Demands are correctly specifying the agent.OS value is equal to 'Windows_NT'
    ![TaskAgentJob_h.jpg](/.attachments/TaskAgentJob_h-3a1682e6-4d25-4842-8267-ba740db31a09.jpg)
1. Configure the Create Namespace task
    * Select a service connection for the target kubernetes cluster
    * Confirm the namespace yaml file is named correctly the artifact source alias in the path is correct, as identified when adding the build artifact.
    ![TaskCreateNamespace_h.jpg](/.attachments/TaskCreateNamespace_h-407624b7-defb-462d-a148-300912609e63.jpg)
1. Configure the replace tokens in config file task
    * Confirm the path to the config folder, published by the build step has the correct Source alias
    ![TaskReplaceTokens_h.jpg](/.attachments/TaskReplaceTokens_h-91e44aef-acee-477c-a3da-950db12c6576.jpg)
1. Configure the Create Web.Config ConfigMap task
    * Select a service connection for the target kubernetes cluster
    * Ensure the namespace is correct, as identified in the namespace.yaml file
    * Ensure the config map name is correct, as specified in the deployment yaml file
    * Ensure the path to the config folder has the correct source alias
    ![TaskCreateConfigMap_h.jpg](/.attachments/TaskCreateConfigMap_h-e4a210a4-e665-45d6-a367-36763c97b75f.jpg)
1. Configure the deploy task
    * Select a service connection for the target kubernetes cluster
    * Ensure the namespace is correct, as identified in the namespace.yaml file
    * Ensure the path to the manifests folder has the correct source alias
    * Ensure the container name is correct, as identified by image name, in the deployment yaml
    ![TaskDeploy_h.jpg](/.attachments/TaskDeploy_h-74c82890-6b3c-447d-b3b2-c8d18150b596.jpg)



