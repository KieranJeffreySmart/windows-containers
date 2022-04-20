# Create Namespace yaml

To isolate resources for your app, on a kubernetes cluster, you can deploy to a namespace. To deploy to the namespace it has to be created first, and so the following yaml should provide a basic template for this.

``` yaml
apiVersion: v1
kind: Namespace
metadata:
  name: my-application
```

You can find more information about Kubernetes Namespaces [here](https://kubernetes.io/docs/concepts/overview/working-with-objects/namespaces/)

# Create Deployment yaml

To deploy your containarized application to a Kubernetes Cluster, you need to create a Deployment yaml file that contains the definitions and configurations of your deployment an it will manage the pods that are being created. The configurations in this file will be applied in the cluster as part of the release process.

The deployment definition contains the name and label for the deployment, the namespace where it's going to be deployed, number of replicas that should be running, the pod definition. 

The following yaml is a typical example of a windows app deployment manifest:

``` yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  labels:
      app: my-application
  name: my-application-deployment
  namespace: my-application
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
        image: tanzu-sandbox-harbor.principality.net/library/my-application
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
            name: my-application-webconfig-configmap
      nodeSelector:
        kubernetes.io/os: windows
      tolerations:
      - key: "windows"
        operator: "Equal"
        value: "2019"
        effect: "NoSchedule"
```
> The image name has been deliberately left without a tag, tanzu-sandbox-harbor.principality.net/library/my-application. This is important as the azure devops will append the relevant build id, as a tag during the release.

> replicas will identify the number of instances of your application that should be running at any one time. Typically this would be equivalent to the number of nodes available, but can be adjusted according to your applications needs

> nodeSelector and tolorations are used to ensure your app is only deployed to Windows nodes, there will always be at least 1 linux node in your cluster to run management services, so this is always needed when deploying a windows app

> The volume config-volume and its respective volume mount is used to inject custom configuration files for each environment in the release pipeline. The ConfigMap my-application-webconfig-configmap, in this case, represents a folder containing one or more configuration files and will be copied to each container that mounts this volume.

You can find more information about Kubernetes Deployments [here](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)

You can find more information about Kubernetes ConfigMaps [here](https://kubernetes.io/docs/concepts/configuration/configmap/) 

# Load Balancer

A service is used to allow network access to a set of pods.
Currently the expectation is that each app that exposes an API or web front end, should have a service of the type LoadBalancer, and a static IP Address to ensure it can be accessed from outside of the cluster

``` yaml
    apiVersion: v1
    kind: Service
    metadata:
    name: my-application-svc
    namespace: my-application
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
  ```

 You can find more information about Kubernetes services and Load Balancers in [here](https://kubernetes.io/docs/concepts/services-networking/service/)