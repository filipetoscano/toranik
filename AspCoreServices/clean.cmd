@echo off

docker ps -a | grep -v IMAGE | awk '{print $1}' | xargs --no-run-if-empty docker rm
docker images | grep -v -E "microsoft|REPOSITORY" | awk '{print $3}' | xargs --no-run-if-empty docker rmi
