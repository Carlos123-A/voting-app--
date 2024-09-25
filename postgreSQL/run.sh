#!/bin/bash

docker run -d \
  --name db \
  --network my-network \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=yourpassword \
  -e POSTGRES_DB=mydb \
  -p 5432:5432 \
  postgres:alpine

