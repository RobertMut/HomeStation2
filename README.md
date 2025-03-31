Links:
- [:point_right: Demo](https://mutnianski.dev/homestation/)
- [:newspaper: Blog post](https://mutnianski.dev/portfolio/2025/03/24/HomeStation.html)

# Description
IoT-based measurement solution of environmental parameters in the home environment using ESP-IDF, MQTT, ASP.NET, Angular.
The main purpose of this project is to collect data from sensors, send using MQTT protocol and display it in a web application.

Also, continuation of my [Engineer's thesis](https://github.com/RobertMut/HomeStation), that I managed to expand with new functionalities.

## Technologies used
### API:
- .Net 8
- ASP.Net Core
- Entity Framework Core
- MQTTNet
- Scrutor
- Docker
### Front-end:
- Angular
- Angular Material
- Chart.js with zoom extension
- Docker
### Device:
- C/C++
- ESP-IDF
- PlatformIO
### Infrastructure
- Microsoft SQL Server
- Docker Compose
- (Optional) Kubernetes
- ESP32-WROOM

# Getting started - Docker launch
## Prerequisites
- Install [Docker](https://docs.docker.com/get-started/get-docker/)
- Clone repository `git clone https://github.com/RobertMut/HomeStation2.git`
- Set up device
- Prepare `compose.yaml` file
    ```yaml
    volumes:
      sqlserver_data:
    networks:
        homestation:
            driver: bridge
    
    services:
        homestation_db:
            container_name: homestationDb
            image: mcr.microsoft.com/mssql/server:2022-latest
            environment:
                - SA_PASSWORD=<AwesomePassword> #Or use secret https://docs.docker.com/compose/how-tos/use-secrets/#use-secrets
                - ACCEPT_EULA=Y
            ports:
                - "1433:1433"
            networks:
                - homestation
            volumes:
                - sqlserver_data:/var/opt/mssql
            restart: always
            
        homestation_api:
            container_name: homestationApi
            environment:
                - ASPNETCORE_HTTP_PORTS=80
                - Database__ConnectionString=Data Source=localhost,1433;Database=homestation;User Id=sa;Password=<AwesomePassword>;Encrypt=False;TrustServerCertificate=True #Or use secret
            build:
                context: ./Web
                dockerfile: Dockerfile
            ports:
                - "1883:1883"
                - "9180:80"
            networks:
                - homestation
            depends_on:
                homestation_db:
                    condition: service_started
            restart: always
        
        homestation_web:
            container_name: homestationWeb
            build:
                context: ./Web/web.client
                args:
                    - HREF=/homestation/
            ports:
                - "9080:80"
                - "9443:443"
            depends_on:
                - homestation_api
            networks:
                - homestation
            restart: always
    ```
## Run
- Run `docker compose -f compose.yaml up -d` in the same directory as `compose.yaml`
- Wait for the containers to start
- Open `http://localhost:9080/homestation/` in your browser
- You should be welcomed with this page:
  ![](https://mutnianski.dev/assets/images/homestation2/homestation2_emptystate.png)

# Getting started - Kubernetes
## Prerequisites
- Install kubernetes
- Create registry (skip if already installed)
- [Install ingress-nginx](https://kubernetes.github.io/ingress-nginx/deploy/) with parameter `--set controller.extraArgs.tcp-services-configmap=ingress-nginx/tcp-services`
- Clone repository `git clone https://github.com/RobertMut/HomeStation2.git`
- Set up device
- Create namespace `kubectl create namespace homestation`
- Prepare [tcp-services ConfigMap](./Miscellaneous/ingress-nginx.yaml)
  ```yaml
  apiVersion: v1
  kind: ConfigMap
  metadata:
    name: tcp-services
    namespace: ingress-nginx
  data:
    1883: "homestation/homestationapi:1883" #Customize MQTT port, should be the same as service port
  ```
- Apply ConfigMap `kubectl apply -f Miscellaneous/ingress-nginx.yaml`
- Run commands (or patch manually). If you want other MQTT port than default, should specify it under `"containerPort": 1883` and `"port": 1883` in the patch command 
```shell
kubectl patch deployment ingress-nginx-controller -n ingress-nginx --patch '{ "spec": { "template": { "spec": { "containers": [ { "name": "controller", "ports": [ { "name": "mqtt", "containerPort": 1883, "protocol": "TCP" } ] } ] } } } }'
kubectl patch service ingress-nginx-controller -n ingress-nginx --patch '{ "spec": { "ports": [ { "appProtocol": "mqtt", "name": "mqtt", "nodePort": 30843, "port": 1883, "protocol": "TCP", "targetPort": "mqtt" } ] } }
```
- Prepare secrets file (skip if you don't want to use secrets):
  ```yaml
  #Secrets are encrypted using base64
  apiVersion: v1
  kind: Secret
  metadata:
    name: homestation-secrets
    namespace: homestation
  data:
    #Data Source=homestationDb,1433;Database=homestation;User Id=sa;Password=<AwesomePassword>;Encrypt=False;TrustServerCertificate=True
    ConnectionString: RGF0YSBTb3VyY2U9aG9tZXN0YXRpb25EYiwxNDMzO0RhdGFiYXNlPWhvbWVzdGF0aW9uO1VzZXIgSWQ9c2E7UGFzc3dvcmQ9PEF3ZXNvbWVQYXNzd29yZD47RW5jcnlwdD1GYWxzZTtUcnVzdFNlcnZlckNlcnRpZmljYXRlPVRydWU= 
    #AwesomePassword
    DbPassword: QXdlc29tZVBhc3N3b3Jk
  ```
- Edit database [deployment file](./Miscellaneous/homestationdb-prepare.yaml)
  ```yaml
  apiVersion: v1
  kind: PersistentVolume
  metadata:
    name: sqlserver-data
    namespace: homestation
  spec:
    accessModes:
      - ReadWriteOnce
    capacity:
      storage: 5Gi
    hostPath:
      path: "/var/opt/mssql"
  ---
  kind: PersistentVolumeClaim
  apiVersion: v1
  metadata:
    name: sqlserver-claim
    namespace: homestation
  spec:
    accessModes:
      - ReadWriteOnce
    resources:
      requests:
        storage: 5Gi
  ---
  apiVersion: apps/v1
  kind: Deployment
  metadata:
    annotations:
      kompose.cmd: kompose -f compose.yaml convert
      kompose.version: 1.35.0 (9532ceef3)
    labels:
      app.kubernetes.io/name: homestationdb-deployment
    name: homestationdb-deployment
    namespace: homestation
  spec:
    replicas: 1
    selector:
      matchLabels:
        app.kubernetes.io/name: homestationdb
    strategy:
      type: Recreate
    template:
      metadata:
        annotations:
          kompose.cmd: kompose -f compose.yaml convert
          kompose.version: 1.35.0 (9532ceef3)
        labels:
          app.kubernetes.io/name: homestationdb
      spec:
        containers:
          - name: homestationdb
            image: mcr.microsoft.com/mssql/rhel/server:2022-latest
            ports:
              - name: "1433port" #Customize SQL Server port
                containerPort: 1433
              - name: "1434port" #Customize SQL Server port
                containerPort: 1434
            env:
              - name: ACCEPT_EULA
                value: "Y"
              - name: SA_PASSWORD
                valueFrom:
                  secretKeyRef:
                    key: DbPassword
                    name: homestation-secrets #Specify other secret if needed, or remove everything under valueFrom: and replace with `value: "<your password>"`
            volumeMounts:
              - name: sqlserver-data
                mountPath: "/var/opt/mssql"
        volumes:
          - name: sqlserver-data
            persistentVolumeClaim:
              claimName: sqlserver-claim
        restartPolicy: Always
  ---
  apiVersion: v1
  kind: Service
  metadata:
    annotations:
      kompose.cmd: kompose -f compose.yaml convert
      kompose.version: 1.35.0 (9532ceef3)
    labels:
      app.kubernetes.io/name: homestationdb
    name: homestationdb
    namespace: homestation
  spec:
    ports:
      - name: "1433port" #Customize SQL Server port
        port: 1433 #Used for communication between pods, ingresses etc.
        targetPort: 1433 #Same as container port
        protocol: TCP
      - name: "1434port" #Customize SQL Server port
        port: 1434 #Used for communication between pods, ingresses etc.
        targetPort: 1434 #Same as container port
        protocol: UDP
    selector:
      app.kubernetes.io/name: homestationdb
  ```
- Apply deployment file `kubectl apply -f Miscellaneous/homestationdb-deployment.yaml`
- Run database preparation job - `kubectl apply -f Miscellaneous/homestationdb-prepare.yaml`
  ```yaml
  apiVersion: batch/v1 #This job should be run after the database is up
  kind: Job
  metadata:
    name: homestationdb-prepare
    namespace: homestation
  spec:
    template:
      spec:
        containers:
          - name: homestationdb-prepare
            image: mcr.microsoft.com/mssql-tools
            command: ["/opt/mssql-tools/bin/sqlcmd"]
            env:
              - name: DbPassword
                valueFrom:
                  secretKeyRef:
                    key: DbPassword
                    name: homestation-secrets #Specify secret name or replace with `value: "<your password>"`, after removing valueFrom:
            args: [ "-S", "homestationDb", "-U", "sa", "-P", "$(DbPassword)", "-C", "-I", "-Q", "IF NOT EXIST (SELECT * FROM sys.databases WHERE name = 'homestation') BEGIN CREATE DATABASE homestation; END;" ]
        restartPolicy: Never
    backoffLimit: 4
  ```
- Edit compose.yaml file
```yaml
volumes:
  sqlserver_data:
networks:
  homestation:
    driver: bridge

services:
  homestation_db:
    container_name: homestationDb
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - SA_PASSWORD=<AwesomePassword>
      - ACCEPT_EULA=Y
    ports:
      - "1433:1433"
    networks:
      - homestation
    volumes:
      - sqlserver_data:/var/opt/mssql
    restart: always

  homestation_api:
    container_name: homestationApi
    environment:
      - ASPNETCORE_HTTP_PORTS=80
      - Database__ConnectionString=Data Source=localhost,1433;Database=homestation;User Id=sa;Password=<AwesomePassword>;Encrypt=False;TrustServerCertificate=True
    build:
      context: ./Web
      dockerfile: Dockerfile
    ports:
      - "1883:1883"
      - "8180:80"
    networks:
      - homestation
    depends_on:
      homestation_db:
        condition: service_started
    restart: always

  homestation_web:
    container_name: homestationWeb
    build:
      context: ./Web/web.client
      args:
        - HREF=/homestation/ #If you want to use /homestation/ suffix to address otherwise replace with /
    ports:
      - "8080:80"
      - "8443:443"
    depends_on:
      - homestation_api
    networks:
      - homestation
    restart: always
```
- Build images - `docker-compose -f compose.yaml build`
- Tag two images (homestation2-homestation_api, homestation2-homestation_web) with prefix to your registry
- Push images to registry e.g `docker push <address:port>/homestation2-homestation_api:<your tag>`, `docker push <address:port>/homestation2-homestation_web:<your tag>`
- Edit [api deployment file](./Miscellaneous/homestationapi-deployment.yaml)
  ```yaml
  apiVersion: apps/v1
  kind: Deployment
  metadata:
    annotations:
      kompose.cmd: kompose -f compose.yaml convert
      kompose.version: 1.35.0 (9532ceef3)
    name: homestationapi-deployment
    namespace: homestation
  spec:
    replicas: 1
    selector:
      matchLabels:
        app.kubernetes.io/name: homestationapi
    template:
      metadata:
        labels:
          app.kubernetes.io/name: homestationapi
      spec:
        containers:
          - env:
              - name: MQTT__Port
                value: "1883" #Pod Internal MQTT Port
              - name: MQTT__Address
                value: "0.0.0.0" #Listen on all
              - name: ASPNETCORE_HTTP_PORTS
                value: "80" #Pod internal HTTP port
              - name: Database__ConnectionString
                valueFrom:
                  secretKeyRef:
                    key: ConnectionString
                    name: homestation-secrets #Specify other secret if needed, or remove everything under valueFrom: and replace with `value: "<your connection string>"`
            image: <registry ip:registry port>/homestation2-homestation_api:<registry tag> #specify image registry
            name: homestationapi
            ports:
              - containerPort: 1883 #Customize MQTT port
                protocol: TCP
              - containerPort: 8180 #Customize HTTP port
                protocol: TCP
        restartPolicy: Always
  ---
  apiVersion: v1
  kind: Service
  metadata:
    labels:
      app.kubernetes.io/name: homestationapi
    name: homestationapi
    namespace: homestation
  spec:
    ports:
      - name: "1883" #Customize MQTT port
        port: 1883 #Service port to communication, should be the same in a ingress
        protocol: TCP
        targetPort: 1883 #Container port should be the same as target port
      - name: "80" #Customize HTTP port, rules same as above
        port: 9180
        protocol: TCP
        targetPort: 80
    selector:
      app.kubernetes.io/name: homestationapi
  ---
  apiVersion: networking.k8s.io/v1
  kind: Ingress
  metadata:
    name: homestationapiingress
    namespace: homestation
    annotations:
      nginx.ingress.kubernetes.io/use-regex: "true"
      nginx.ingress.kubernetes.io/rewrite-target: /api/$1
  spec:
    rules:
      - http:
          paths:
            - path: /api/(.*)
              backend:
                service:
                  name: homestationapi
                  port:
                    number: 9180 #HTTP port
              pathType: ImplementationSpecific
    ingressClassName: nginx 
  ```
- Apply deployment file `kubectl apply -f Miscellaneous/homestationapi-deployment.yaml`
- Prepare [web deployment file](./Miscellaneous/homestationweb-deployment.yaml)
  ```yaml
  apiVersion: apps/v1
  kind: Deployment
  metadata:
    namespace: homestation
    annotations:
      kompose.cmd: kompose -f compose.yaml convert
      kompose.version: 1.35.0 (9532ceef3)
    name: homestationweb-deployment
  spec:
    replicas: 1
    selector:
      matchLabels:
        app.kubernetes.io/name: homestationweb
    template:
      metadata:
        annotations:
          kompose.cmd: kompose -f compose.yaml convert
          kompose.version: 1.35.0 (9532ceef3)
        labels:
          app.kubernetes.io/name: homestationweb
      spec:
        containers:
          - image: <address:port>/homestation2-homestation_web:<your image tag> #specify registry
            name: homestationweb
            env:
              - name: TARGET_URL
                value: "http://homestationApi/homestation/"
            ports:
              - name: "https"
                containerPort: 8443 #Customize HTTPS port - will be deleted in future, ingress configuration uses HTTP
              - name: "http"
                containerPort: 8080 #Customzize HTTP port
        restartPolicy: Always
  ---
  apiVersion: v1
  kind: Service
  metadata:
    namespace: homestation
    annotations:
      kompose.cmd: kompose -f compose.yaml convert
      kompose.version: 1.35.0 (9532ceef3)
    labels:
      io.kompose.service: homestationweb
    name: homestationweb
  spec:
    ports:
      - name: "http" #customize HTTP port
        port: 8080 #Service port to communication, should be the same in a ingress
        protocol: TCP
        targetPort: 80
      - name: "https" #customize HTTPS port
        port: 8443 #Service port to communication, should be the same in a ingress
        protocol: TCP
        targetPort: 443
    selector:
      app.kubernetes.io/name: homestationweb
  ---
  apiVersion: networking.k8s.io/v1
  kind: Ingress
  metadata:
    name: homestationwebingress
    namespace: homestation
    annotations:
      nginx.ingress.kubernetes.io/use-regex: "true"
      nginx.ingress.kubernetes.io/rewrite-target: /homestation/ #Same as in compose.yaml
  spec:
    rules:
      - http:
          paths:
            - path: /homestation/ #Same as in compose.yaml, replace with / if you don't want to use suffix
              backend:
                service:
                  name: homestationweb
                  port:
                    number: 8080 #Exposed HTTP port, should be the same as service port
              pathType: Prefix
    ingressClassName: nginx
  ```
- Apply deployment file `kubectl apply -f Miscellaneous/homestationweb-deployment.yaml`

# Device Set-up
## Prerequisites
- [Install Visual Studio Code](https://code.visualstudio.com/)
- [Install PlatformIO](https://platformio.org/)
- [Install ESP-IDF](https://idf.espressif.com/) (if not installed with PlatformIO)
- If you use ESP32-WROOM development board
  - Connect the sensors (Plantower PMS 3003, Bosch BME280) to device
- If you use own device
  - Setup MQTT client address and port to HomeStation's API
  - Send data using this interface
    ```json
    {
      "deviceid": 0, //the device id number,
      "temperature": 0.0, //double e.g 24.9,
      "humidity": 0.0, //double e.g 100321.6,
      "pressure": 0.0, //double e.g 44.4,
      "pm1_0": "0", //uint e.g 9,
      "pm2_5": "0", //uint e.g 1,
      "pm10": "0", //uint e.g 2
    }
    ```