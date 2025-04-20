# PlanetsSatellites Microservices Project

This repository demonstrates a microservices-based architecture built with .NET, Docker, Kubernetes, RabbitMQ, and gRPC. It is designed as a practical showcase of modern backend technologies and container orchestration. The project can be run locally, in Docer or in K8s.

**Project Overview**
The system consists of two microservices:

1. PlanetService: Manages planetary data.
2. SatelliteService: Manages satellite data linked to planets.

These services communicate using:

- http for inter-service communication.
- gRPC for efficient inter-service communication.
- RabbitMQ for asynchronous message passing.

**Features**
- Microservices Architecture: Decoupled services for scalability and modularity.
- gRPC Communication: High-performance communication between services.
- Asynchronous Messaging: RabbitMQ ensures reliable event handling.
- Kubernetes Orchestration: Deploy services into a local K8s cluster.
- Containerization: Docker images available for both services.

  **Details**:
  - PlanetService works with DB in-memory in DEV environment and with MS SQL Server in PROD env. If there are no planets in the DB we do the [seeding]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/PlanetService/Data/DbSeeder.cs)) in start of the service.
  - SatelliteService works with DB in-memory DB in all cases.
  - [PlanetController]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/PlanetService/Controllers/PlanetController.cs)) from PlanetService can: GetAll, GetById и Create. When we create a new planet we notify SatelliteService synchoniuslly by http, see [HttpDataClient]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/PlanetService/SyncDataServices/Http/HttpDataClient.cs)).
  - Also PlanetService send the info about new planet to SatelliteService [asynchoniuslly by message bus, using RabbitMQ]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/PlanetService/AsyncDataServices/MessageBusDataClient.cs)) and the planet creates on SattelliteService side too.
  - SatelliteService has endpoints for: test inbound calls from PlanetService; getting all planets (objects of the SatelliteService that stored in memory DB); getting all satellites for certain planet; getting certain satellite for certain planet and creating satellite for certain planet.
  - SatelliteService do the prepopulation by asking PlanetService about existing planets [synchoniuslly by gRPC]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/SatelliteService/Data/PrepDb.cs)).
  - The latest image of planetservice is [here]([url](https://hub.docker.com/r/dgluhotorenko/planetservice)).
  - The latest image of satelliteservice is [here]([url](https://hub.docker.com/r/dgluhotorenko/satelliteservice)).

**Prerequisites**

To run the project, ensure you have installed [Docker]([url](https://docs.docker.com/desktop/)).


**Getting Started in K8s:**

Clone the Repository
>git clone https://github.com/dgluhotorenko/PlanetsSatellites.git

Add next row to your hosts file:
>127.0.0.1 planetsatellitessevice.com

Go to ~\PlanetsSatellites\K8S

Create a PersistentVolumeClaim (PVC) in K8s to store our DB.
>kubectl apply -f local-pvc.yaml

Create a K8s Secret to store the MS SQL SA password:
>kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"

Deploy the MS SQL Server instance and its services:
>kubectl apply -f mssql-planet-depl.yaml

Set up the NGINX Ingress Controller in the K8s cluster to manage external traffic routing to services within the cluster:
>kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0-beta.0/deploy/static/provider/cloud/deploy.yaml

Configure an Ingress to route external requests to the microservices:
>kubectl apply -f ingress-srv.yaml

Deploy the planet microservice and its internal service:
>kubectl apply -f planet-depl.yaml

Deploy the satellite microservice and its internal service:
>kubectl apply -f satellite-depl.yaml

Deploy RabbitMQ and its services:
>kubectl apply -f rabbitmq-depl.yaml


That's it.
Now you can get all planets by calling planet service in K8s claster by sending GET request to http://planetsatellitessevice.com/api/planet in Postman (or something like this).
You should see the json with result:

![image](https://github.com/user-attachments/assets/c72e7366-423a-47d9-94ac-a52457e6417e)

To get one planet by id call please
>http://planetsatellitessevice.com/api/planet/ID_HERE

To create a new planet send please POST request with body like this 

>{
>    "name": "Test planet",
>    "mass": 1,
>    "radius": 1000
>} 

to endpoint
>http://planetsatellitessevice.com/api/planet

You should see something like this as a result:

![image](https://github.com/user-attachments/assets/c6da3c12-cca2-4dac-9b28-9a788f6d6d1d)

After successful creation a new planet, the planet service send sync message by http to satellite service (that uses only for showing the message in console) and async message via RabbitMQ message bus which create a planet object on SatelliteService side. To check it, you can send GET request to 
>http://planetsatellitessevice.com/api/s/planet

it should return the info about newly created planet. 
Also in this response you can see all other planets because they were created when we run satellite service by requesting them from the planet service via gRPC (see [PrepDb]([url](https://github.com/dgluhotorenko/PlanetsSatellites/blob/main/SatelliteService/Data/PrepDb.cs))).

Now we can create a satellite for the planet (on SatelliteService side). We can do it by calling
>http://planetsatellitessevice.com/api/s/planets/ID_OF_PLANET/satellite

As a result you should get something like this:

![image](https://github.com/user-attachments/assets/faa764cc-e965-4a09-9cd4-681208c09bd8)

You can get all sattelites of your planet has by call
>http://planetsatellitessevice.com/api/s/planets/ID_OF_PLANET/satellite

Or you can get the certain sattelite of the planet by call
>http://planetsatellitessevice.com/api/s/planets/ID_OF_PLANET/satellite/ID_OF_SATELLITE


**When you finish you can clean all:**


>kubectl delete deployment satellite-depl

>kubectl delete deployment planet-depl

>kubectl delete deployment rabbitmq-depl

>kubectl delete deployment mssql-planet-depl

>kubectl delete service planet-clusterip-service

>kubectl delete service satellite-clusterip-service

>kubectl delete service mssql-clusterip-service

>kubectl delete service mssql-loadbalancer-service

>kubectl delete ingress ingress-service

>kubectl delete service rabbitmq-clusterip-service

>kubectl delete service rabbitmq-loadbalancer-service
