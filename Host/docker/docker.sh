#!/bin/bash

cp -r ../[Bb]in/[Dd]ebug bin

docker build -t wcfhost-lgb .

rm -rf bin

docker run -itd --rm -p 8080:8080 --name HostLGB wcfhost-lgb