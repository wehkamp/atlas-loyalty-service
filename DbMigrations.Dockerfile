# This is a lightweight dockerfile for the DatabaseMigrator CLI tool, used locally (and potentially later as a separate
# initContainer). This was created separately from the main Dockerfile to have a faster startup: not running the tests
# and building the main Fraud service project.
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
ARG tag
ARG VERBOSITY="minimal"

WORKDIR /app

ENV TZ=Europe/Amsterdam
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=0

# copy global files to restore
COPY *.sln *.config ./

# copy src files to restore
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# copy test files to restore
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done;

RUN dotnet restore --verbosity "$VERBOSITY" Fraud.sln || exit 1

# copy dirs that are only needed for building and testing
COPY src ./src

# Publish DatabaseMigrator CLI tool for initContainer to invoke (localhost development setup)
RUN dotnet publish ./src/Loyalty.DatabaseMigrator/Loyalty.DatabaseMigrator.csproj -c Debug -o dbmigrator

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG tag
WORKDIR /app
ENV TZ=Europe/Amsterdam
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_ENVIRONMENT=Development
# Note: the next copy command overrides shared dependencies. This is by design instead of adding a new subdirectory with another 130 megabytes
COPY --from=build-env /app/dbmigrator .
ENTRYPOINT ["dotnet", "Loyalty.DatabaseMigrator.dll"]
