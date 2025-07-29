# REQUIREMENTS
dotnet sdk 9.0 or later

## INSTALL ASPIRE CLI
dotnet tool install --global aspire.cli --prerelease

## GENERATE DOCKER FILE
cd UniversalMapper.AppHost
aspire publish -o ../deploy/unimap

Change the .env file to match your environment variables.
Change the docker-compose.yml file to map your custom networks and volumes.