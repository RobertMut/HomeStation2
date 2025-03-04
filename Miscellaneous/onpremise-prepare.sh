#!/bin/bash

echo "UFW - allow connection"
sudo ufw allow 6443/tcp #apiserver
sudo ufw allow from 10.42.0.0/16 to any #pods
sudo ufw allow from 10.43.0.0/16 to any #services

echo "Install k3s without traefik"
sudo curl -sfL https://get.k3s.io | INSTALL_K3S_EXEC="--disable=traefik" sh -

echo "Create image registry"
mkdir ./data
chmod -R +xr ./data
echo "{ \
        ""insecure-registries"": [], \
        ""registry-mirrors"": [], \
        ""insecure-registries"": [""127.0.0.1:5000""] \
      }" > /etc/docker/daemon.json
echo "version: 0.1
      log:
        level: info
        formatter: json
        fields:
          service: registry
      storage:
        cache:
          layerinfo: inmemory
        filesystem:
          rootdirectory: /var/lib/registry
      http:
        addr: :5000" > /etc/docker/registry/config.yml
sudo systemctl restart docker
docker compose -f image-registry-compose.yaml up

echo "Build image"
git clone https://github.com/RobertMut/HomeStation2.git
cd ./HomeStation2
docker compose -f compose.yaml build 

echo "Push image"
cd ./Miscellaneous
docker compose -f ../compose.yaml config --images | grep 'homestation' | awk '{print $1}' | while read image; do docker tag $image 127.0.0.1:5000/$image; done
docker compose -f ../compose.yaml config --images | grep 'homestation' | awk '{print $1}' | while read image; do docker push 127.0.0.1:5000/$image; done

echo "Permissions to deploy"
chmod +x ./deploy.sh

echo "Creating /var/opt/mssql"
mkdir /var/opt/mssql
chmod -R 766 /var/opt/mssql

sh ./deploy.sh