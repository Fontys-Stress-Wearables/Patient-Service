# Patient-Service

## Introduction
The patient service is a service made by Group-1 for the SWSP project.
The task of this service is to manage the patients in the SWSP system.

## Build steps

### development
To run the project locally you can run `docker compose up`.
This will build the API and run all the services necessary for it to function properly.
If there is no NATS service running you can start it by first running  `docker compose -f docker-compose-nats.yaml up -d`.

## Docker
If you want to manually build a Docker image and push said image, use the following commands:
- build the image for the backend by running `docker build -t ghcr.io/fontys-stress-wearables/patient-service:main .`
- Push the image to the docker registry by running `docker push 
ghcr.io/fontys-stress-wearables/patient-service:main `
