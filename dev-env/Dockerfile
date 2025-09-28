# [Choice] .NET version: 7.0, 6.0, 5.0, 3.1
# Other variants: 6.0-bullseye, 5.0-bullseye, 3.1-bullseye, 6.0-focal, 5.0-focal, 3.1-focal
# The first line determines the version of .NET that will be used
ARG VARIANT="8.0"
FROM mcr.microsoft.com/devcontainers/dotnet:${VARIANT}


# [Choice] Node.js version: none, lts/*, 16, 14, 12, 10
ARG NODE_VERSION="none"
RUN if [ "${NODE_VERSION}" != "none" ]; then \
      su vscode -c "umask 0002 && . /usr/local/share/nvm/nvm.sh && nvm install ${NODE_VERSION} 2>&1"; \
    fi

# [Optional] OS packages
RUN apt-get update && \
    export DEBIAN_FRONTEND=noninteractive && \
    apt-get -qy full-upgrade && \
    apt-get install -qy curl && \
    apt-get -y install --no-install-recommends vim && \
    curl -sSL https://get.docker.com/ | sh

# Install the EF Core command-line tools
RUN dotnet tool install -g dotnet-ef --version 8

# Add dotnet tools to PATH
ENV PATH $PATH:/root/.dotnet/tools

# Configure for HTTPS development
RUN dotnet dev-certs https
