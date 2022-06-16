# Signingserver



## Configuration

appsettings.json

## Configuration Path

/var/config/signing/appsettings.json

## Installation

$ docker build -t [repository]:[tag] .

$ docker run -d -p 8080:80 [repository]:[tag]

## Logs

$ docker ps //To check container ID

$ docker logs [container ID]

# MongoDb

## Create Database

$ use PKI_SIGN

## Create Collection

$ db.createCollection("ExternalClient")

$ db.createCollection("SigningRequest")

## Create Credentials

$ db.ExternalClient.insert({name: "Pradeep", clientid:  "{clientid}", secret: "{secret}"})

$ db.ExternalClient.find() //To Check