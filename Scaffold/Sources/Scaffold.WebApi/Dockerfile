FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
ENV ASPNETCORE_HEALTHCHECKPORT=8081
ENV ASPNETCORE_URLS=http://+:80;http://+:8081
EXPOSE 80 8081

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY . .
WORKDIR /src/Sources/Scaffold.WebApi
RUN dotnet build Scaffold.WebApi.csproj --configuration Release

FROM build AS publish
RUN dotnet publish Scaffold.WebApi.csproj --configuration Release --no-build --output /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Scaffold.WebApi.dll"]
