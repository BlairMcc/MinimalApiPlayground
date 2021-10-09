# .Net 6 Minimal API Playground
A playground app to explore the following technologies
* .Net 6 Minimal Apis
* Fluent Validation
* Microsoft SQL Server in Linux container
* Docker Dev Environments

# How to Build
## Docker
### Docker Dev Environments
1. Use the repo URL - https://github.com/BlairMcc/MinimalApiPlayground
2. Once created Open it in VS Code
3. Open Terminal (Ctrl+~), run 'docker compose up -d' (Docker Desktop does not do this, I'm unsure why)

See here for more information - https://docs.docker.com/desktop/dev-environments/

### Docker Compose
1. Navigate to root of repo in cmdline 
2. Run 'docker compose up -d'

###
Resources
* API - http://localhost:8081/
* Swagger - http://localhost:8081/swagger/index.html
* Microsoft SQL Server - localhost,1431

## Local
1. Run MinimalAPI from Visual Studio

Note that this assumes SQL Server exists on localhost with Windows Auth enabled 

# Help
## API errored on docker compose
It is likely the API has started before SQL Server is fully healthy. Giving the API a nudge should get everything up and running. Getting a reliable health check on a SQL Server container has proven to be tricky.

# Next Steps
* Better separate concerns, though it is neat that all logic is in <150 LoC
* Add ability to toggle using an in-memory database or docker compose
* Running docker-compose from Visual Studio does not work as expected, it could be some unexpected port mappings