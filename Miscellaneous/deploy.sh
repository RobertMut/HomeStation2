#!/bin/bash

echo "Deploying homestation"
kubectl apply -f ./homestationdb-deployment.yaml
sleep 20
kubectl apply -f ./homestationdb-prepare.yaml
kubectl apply -f ./homestationapi-deployment.yaml,./homestationweb-deployment.yaml -n homestation