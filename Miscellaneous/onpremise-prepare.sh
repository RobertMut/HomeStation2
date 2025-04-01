#!/bin/bash
# -------------------------------------------
# On-premise deployment file for personal use
# -------------------------------------------

helpFunction()
{
   echo ""
   echo "Usage: $0 -c [true/false] -r [true/false] -p [true/false]"
   echo -e "\t-c Enables git clone of current master"
   echo -e "\t-r Create docker registry"
   echo -e "\t-p Install and/or override nginx files"
   exit 1 # Exit script after printing help
}

while getopts "c:r:p:" opt
do
   case "$opt" in
      c ) clone="$OPTARG" ;;
      r ) createRegistry="$OPTARG" ;;
      p ) prepareNginx="$OPTARG" ;;
      ? ) helpFunction ;;
   esac
done

if [ -z "$clone" ] || [ -z "$createRegistry" ] || [ -z "$prepareNginx" ]
then
   echo "Some or all of the parameters are empty";
   helpFunction
fi
cd ~
echo "UFW - allow connection"
sudo ufw allow 6443/tcp #apiserver
sudo ufw allow from 10.42.0.0/16 to any #pods
sudo ufw allow from 10.43.0.0/16 to any #services

echo "Check K3s installation.."
if ! command -v k3s 2>&1 >/dev/null
then
  echo "Installing K3s without traefik"
  sudo curl -sfL https://get.k3s.io | INSTALL_K3S_EXEC="--disable=traefik --disable=metrics-server" sh -
fi

if ! command -v docker 2>&1 >/dev/null
then
  echo "Docker install.."
  sudo apt-get update
  sudo apt-get install ca-certificates curl
  sudo install -m 0755 -d /etc/apt/keyrings
  sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
  sudo chmod a+r /etc/apt/keyrings/docker.asc
  
  echo \
    "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
    $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
    sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
  sudo apt-get update
  
  sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
fi

if [ "$createRegistry" = true ];
then
  mkdir ./data
  chmod -R +xr ./data
  if ! grep -q "127.0.0.1" /etc/docker/daemon.json;
  then 
    printf '%s\n' '{' \
            '"insecure-registries": [],'\
            '"registry-mirrors": [],' \
            '"insecure-registries": ["127.0.0.1:5000"]' \
          '}' > /etc/docker/daemon.json
  fi
  
  if ! grep -q ":5000" /etc/docker/registry/config.yml;
  then 
    cp ./docker_registry_config.yml /etc/docker/registry/config.yml
  fi
  
sudo systemctl stop docker
systemctl daemon-reload
sudo systemctl start docker
  
fi
if [ "$clone" = true ]; 
then
  echo "Cloning..."
  rm -rf ./HomeStation2
  git clone https://github.com/RobertMut/HomeStation2.git
fi

if [ "$createRegistry" = true ];
then
  echo "Preparing registry.."
  docker compose -f ./HomeStation2/Miscellaneous/image-registry-compose.yaml up -d
fi

cd ./HomeStation2

if [ "$clone" = true ]; 
then
  echo "Build image"
  #Delete old local images
  docker images --all | grep 'homestation' | awk '{print $3}' | sort -u | while read image; do docker rmi --force $image; done
  #Build new
  docker compose -f compose.yaml build
  echo "Push image"
  #Tagging
  docker images | grep 'homestation' | awk '{print $3,$1}' | while IFS=" " read id name; do docker tag $id 127.0.0.1:5000/$name:latest; done;
  #Push
  docker images | grep -E -i '^127.0.0.1:5000*' | grep 'homestation' | awk '{print $1}' | while read image; do docker push $image:latest; done 
fi

cd ./Miscellaneous

echo "Permissions to deploy"
chmod +x ./deploy.sh

echo "Creating /var/opt/mssql"
mkdir /var/opt/mssql
chmod -R 766 /var/opt/mssql

if [ "$prepareNginx" = true ];
then
  echo "Prepare nginx"
  export KUBECONFIG=/etc/rancher/k3s/k3s.yaml
  curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3
  chmod 700 get_helm.sh
  ./get_helm.sh
  helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
  helm repo update
  kubectl apply -f ./ingress-nginx.yaml
  kubectl create namespace nginx-ingress
  helm upgrade --install ingress-nginx ingress-nginx \
    --repo https://kubernetes.github.io/ingress-nginx \
    --namespace ingress-nginx --create-namespace \
    --set controller.extraArgs.tcp-services-configmap=ingress-nginx/tcp-services \
    --set controller.allowSnippetAnnotations=true
  kubectl patch deployment ingress-nginx-controller -n ingress-nginx --patch '{ "spec": { "template": { "spec": { "containers": [ { "name": "controller", "ports": [ { "name": "mqtt", "containerPort": 1883, "protocol": "TCP" } ] } ] } } } }'
  kubectl patch service ingress-nginx-controller -n ingress-nginx --patch '{ "spec": { "ports": [ { "appProtocol": "mqtt", "name": "mqtt", "nodePort": 30843, "port": 1883, "protocol": "TCP", "targetPort": "mqtt" } ] } }'
fi

kubectl create namespace homestation
sh ./deploy.sh
