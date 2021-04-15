#!/bin/bash

#docker pull mcr.microsoft.com/dotnet/framework/wcf:4.8

cp -r ../[Bb]in bin

docker build -t wcfhost .

rm -rf bin

docker run -itd -p 8080:8080 --name Host wcfhost

#docker inspect -f="{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" Host