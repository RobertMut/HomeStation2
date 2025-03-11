#!/bin/bash

echo "Deploying homestation"
kubectl apply -f ./homestationdb-deployment.yaml,./homestationapi-deployment.yaml,./homestationweb-deployment.yaml -n homestation