FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
ARG TAG
ARG VERBOSITY="minimal"

# Install jq
RUN apt-get update && \
    apt-get install -y jq && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

ENV TZ=Europe/Amsterdam
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=0

# Copy global files to restore
COPY *.sln *.config *.ruleset *.props ./

# Copy src files to restore
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

# Copy test files to restore
COPY test/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p test/${file%.*}/ && mv $file test/${file%.*}/; done

# Restore dependencies for both solutions
RUN dotnet restore --verbosity "$VERBOSITY" || exit 1

# Copy dirs that are only needed for building and testing
COPY src ./src
COPY test ./test

# Note on build: don't use --no-restore, sometimes certain packages cannot be
# restored by the dotnet restore. The build will add them, as it has more context (!?)
# example: Package System.Text.Json, version 6.0.0 was not found
#RUN dotnet build --configuration Release --verbosity "$VERBOSITY" -nowarn:NETSDK1004 || exit 1

# Run tests
RUN dotnet test --configuration Debug --logger "console;verbosity=$VERBOSITY" || exit 1

COPY .git ./.git

# Add git information
# Replacing any single forward slash in the author name with dash -
RUN HASH=$(git rev-parse HEAD); \
  BRANCH=$(git branch --remotes --contains $HASH | head -n 1 | xargs echo -n | sed 's/origin\///'); \
  COMMITMESSAGE=$(git log -1 --pretty=format:'%s'); \
  AUTHOR_NAME=$(git log -1 --pretty=format:'%an'); \
  AUTHOR_EMAIL=$(git log -1 --pretty-format:'%ae'); \
  DATE=$(git log -1 --format="%ad" --date=iso); \
  echo '{}' | \
  jq \
   --arg HASH "$HASH" \
   --arg BRANCH "$BRANCH" \
   --arg COMMITMESSAGE "$COMMITMESSAGE" \
   --arg AUTHOR_NAME "$AUTHOR_NAME" \
   --arg AUTHOR_EMAIL "$AUTHOR_EMAIL" \
   --arg DATE "$DATE" \
   --arg TAG "$TAG" \
   '. + { \
    commit: $HASH, \
    branch: $BRANCH, \
    commit_message: $COMMITMESSAGE, \
    author_name: $AUTHOR_NAME, \
    author_email: $AUTHOR_EMAIL, \
    date: $DATE, \
    image_tag: $TAG \
  }' > git.json; cat git.json;

# Run tests & publish each project to its own directory
RUN dotnet publish ./src/Loyalty.Api/Loyalty.Api.csproj -c Release -o /app/out/Loyalty.Api && \
    dotnet publish ./src/Loyalty.DatabaseMigrator/Loyalty.DatabaseMigrator.csproj -c Release -o /app/out/Loyalty.DatabaseMigrator  && \
    dotnet publish ./src/Loyalty.KafkaProcessors/Loyalty.KafkaProcessors.csproj -c Release -o /app/out/Loyalty.KafkaProcessors

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
ARG TAG
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5000
ENV TZ=Europe/Amsterdam
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# Copy project outputs to their respective directories
COPY --from=build-env /app/out/Loyalty.Api ./Loyalty.Api
COPY --from=build-env /app/out/Loyalty.DatabaseMigrator ./Loyalty.DatabaseMigrator
COPY --from=build-env /app/out/Loyalty.KafkaProcessors ./Loyalty.KafkaProcessors

# Copy git.json to each Fraud.* directory
COPY --from=build-env /app/git.json ./Loyalty.Api/git.json
COPY --from=build-env /app/git.json ./Loyalty.DatabaseMigrator/git.json
COPY --from=build-env /app/git.json ./Loyalty.KafkaProcessors/git.json

EXPOSE 5000

# Metadata labels
# Changing any of the values below, you need to manually trigger for all environments the 'monolitic-seed' job in Jenkins CD
LABEL blaze.service.id="oms-core-audit-trail" \
  blaze.service.name="atlas-oms-core-audit-trail-service" \
  blaze.service.version="${TAG}" \
  blaze.service.team="order-management" \
  blaze.service.main-language="dotnet" \
  blaze.service.deployment.promotion.prod.manual-step="false"
