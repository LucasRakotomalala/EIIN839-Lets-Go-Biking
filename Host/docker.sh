#!/bin/bash

# Pull the image first, the one used in Host Dockerfile
#docker pull mcr.microsoft.com/dotnet/framework/wcf:4.8

# Create the image
docker build -t wcfhost .

# Run the container
docker run -itd --name Host wcfhost

# Get IP Address
docker inspect -f="{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" Host