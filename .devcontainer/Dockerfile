FROM mcr.microsoft.com/devcontainers/dotnet:10.0

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
  && apt-get -y install --no-install-recommends postgresql-client

RUN su vscode -c "dotnet tool install dotnet-aspnet-codegenerator --global"
RUN su vscode -c "dotnet tool install dotnet-ef --global"

# [Optional] Uncomment this line to install global node packages.
# RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && npm install -g <your-package-here>" 2>&1