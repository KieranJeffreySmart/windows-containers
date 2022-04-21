# Prerequisites
In order to deploy a Windows application to a Kubernetes cluster your cluster will need a node pool with windows machines.
An image with your application also needs to be in an accessible registry and [Microsoft Container Registry](mcr.microsoft.com) should also be accessible from the cluster, either directly or via a proxy to it, to pull licensed layers during deployment.

To deploy using an Azure DevOps pipeline, you should ensure your build stage has been configured correctly and all manifests can be published for release

To deploy manually, you will need to install a version of [Kubectl](https://kubernetes.io/releases/) compatible with your Kubernetes cluster, and apply the manifests using the `kubectl apply -f my-manifest.yml` command

# Create Namespace yaml

To isolate resources for your app, on a cluster you can deploy to a Kubernetes [Namespace](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/). Namespaces can be defined in yaml and maintained with your source code.

``` yaml
apiVersion: v1
kind: Namespace
metadata:
  name: my-application-namespace
```

# Create Deployment yaml

To deploy your application to a cluster, you should create a Kubernetes [Deployment](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/) that contains the definitions and configurations of your deployment an it will manage the pods that are being created. The configurations in this file will be applied in the cluster as part of the release process.

The definition contains a name for the deployment and a label for application matching, the namespace where it's going to be deployed, number of replicas that should be running and a set of container definitions. 

The following yaml is a typical example of a windows app deployment manifest:

``` yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  labels:
      app: my-application
  name: my-application-deployment
  namespace: my-application-namespace
spec:
  replicas: 3
  selector:
    matchLabels:
      app: my-application
  template:
    metadata:
      labels:
        app: my-application
    spec:
      containers:
      - name: my-application
        image: my-container-registry/my-application
        env:
        - name: PORT
          value: "80"
        ports:
        - name: http
          containerPort: 80
        volumeMounts:
        - name: config-volume
          mountPath: "/config/"
      volumes:
        - name: config-volume
          configMap:
            name: my-application-config
      nodeSelector:
        kubernetes.io/os: windows
      tolerations:
      - key: "windows"
        operator: "Equal"
        value: "2019"
        effect: "NoSchedule"
```
> The image tag is deliberately omitted when using AzureDevOps as the pipeline will append the relevant tag during release. To apply this deployment manually, you may need to add a relevant tag

> nodeSelector and tolerations are used to ensure your app is only deployed to Windows nodes. There will always be at least 1 linux node in your cluster to run management services, so this is always needed when deploying a windows app

> The volume config-volume and its respective volume mount uses a Kubernetes [ConfigMap](https://kubernetes.io/docs/concepts/configuration/configmap/)  to inject custom configuration files for each environment in the release pipeline.

# Load Balancer

A Kubernetes [Service](https://kubernetes.io/docs/concepts/services-networking/service/) is used to allow network access to a set of pods behind a load balancer.

``` yaml
    apiVersion: v1
    kind: Service
    metadata:
    name: my-application-load-balancer
    namespace: my-application-namespace
    labels:
        app: my-application
    spec:
      ports:
          # the port that this service should serve on
      - port: 80
          targetPort: 80
      selector:
          app: my-application
      type: LoadBalancer
      loadBalancerIP: 10.10.10.19
  ```
  > A static IP has been declared using `loadBalancerIP`. This is useful to expose your application under it's own DNS. To use a shared DNS you should implement Kubernetes [Ingress](https://kubernetes.io/docs/concepts/services-networking/ingress/) rules