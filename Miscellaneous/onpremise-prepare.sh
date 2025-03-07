#!/bin/bash
# -------------------------------------------
# On-premise deployment file for personal use
# -------------------------------------------

echo "UFW - allow connection"
sudo ufw allow 6443/tcp #apiserver
sudo ufw allow from 10.42.0.0/16 to any #pods
sudo ufw allow from 10.43.0.0/16 to any #services

echo "Check K3s installation.."
if ! command -v k3s 2>&1 >/dev/null
then
  echo "Installing K3s without traefik"
  sudo curl -sfL https://get.k3s.io | INSTALL_K3S_EXEC="--disable=traefik" sh -
fi

echo "Create image registry"
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
  printf '%s\n' 'version: 0.1'\
        'log:'\
          'level: info'\
          'formatter: json'\
          'fields:'\
            'service: registry'\
        'storage:'\
          'cache:'\
            'layerinfo: inmemory'\
          'filesystem:'\
            'rootdirectory: /var/lib/registry' \
        'http:' \
          'addr: :5000' > /etc/docker/registry/config.yml
fi

sudo systemctl restart docker
docker compose -f image-registry-compose.yaml up

echo "Build image"
rm -rf ./HomeStation2
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

echo "Prepare nginx"
apt-get install nginx -y
printf '%s\n' 'server {' \
          'listen 80 default_server;'\
          'listen [::]:80 default_server;'\
          ''\
          'root /usr/share/nginx/html;'\
          'index index.html;'\
          'location / {'\
              'proxy_pass http://127.0.0.1:30080/;'\
              'proxy_set_header Host $host;'\
              'proxy_set_header X-Real-IP $remote_addr;'\
              'proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;'\
              'return 301 https://$host$request_uri;'\
              'proxy_read_timeout 1800;'\
              'proxy_connect_timeout 1800;'\
              'proxy_send_timeout 1800;'\
              'send_timeout 1800;'\
          '}'\
      '}'\
      'server {'\
          'listen      443 ssl http2 default_server;'\
          'listen      [::]:443 ssl http2 default_server;'\
          'server_name     localhost;'\
          ''\
          'ssl_protocols SSLv3 TLSv1 TLSv1.1 TLSv1.2;'\
          '}' > /etc/nginx/conf.d/default.conf