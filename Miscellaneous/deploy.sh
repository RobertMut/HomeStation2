#!/bin/bash

echo "Deploying homestation"
kubectl apply -f ./ingress-nginx.yaml
kubectl apply -f ./homestationdb-deployment.yaml,./homestationapi-deployment.yaml,./homestationweb-deployment.yaml -n homestation