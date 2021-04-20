#!/bin/bash

cp -r ../[Bb]in/[Dd]ebug bin

docker build -t client-lgb .

rm -rf bin

docker run -itd --rm --name ClientLGB client-lgb