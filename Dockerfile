# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# CDP platform health checks use curl.
USER root
RUN apt update && \
    apt install curl -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Build stage image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY .config/dotnet-tools.json .config/dotnet-tools.json
COPY .csharpierrc .csharpierrc
COPY .editorconfig .editorconfig
COPY Directory.Build.props Directory.Build.props
COPY src/ReverseProxy/ReverseProxy.csproj src/ReverseProxy/ReverseProxy.csproj

RUN dotnet tool restore
RUN dotnet restore src/ReverseProxy/ReverseProxy.csproj

COPY src/ReverseProxy src/ReverseProxy

RUN dotnet csharpier check .
RUN dotnet publish src/ReverseProxy/ReverseProxy.csproj -c Release --no-restore --warnaserror -o /app/publish /p:UseAppHost=false

# Final production image
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_HTTP_PORTS=
ENV PORT=8085

EXPOSE 8085
USER $APP_UID
ENTRYPOINT ["dotnet", "ReverseProxy.dll"]
