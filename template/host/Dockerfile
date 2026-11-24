FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN cd host && dotnet publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /src/host/bin/Release/net10.0/publish .
ENTRYPOINT dotnet Telega.Host.dll