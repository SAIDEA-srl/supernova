# require dotnet SDK 9.0 or later
# INSTALL ASPIRE CLI
dotnet tool install -g Aspire.Cli --pre-release

# build docker compose file
dotnet aspire build -o ../deplyoy/envname

## note: aspire is limited to one network per compose file
## adjust the networks if needed

# run docker compose file
cd ../deploy/envname
docker compose up -d