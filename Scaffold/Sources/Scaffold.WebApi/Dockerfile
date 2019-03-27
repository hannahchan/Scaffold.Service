FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
ENV ASPNETCORE_HEALTHCHECKPORT=8081
ENV ASPNETCORE_SUPPRESSSTATUSMESSAGES=true
ENV ASPNETCORE_URLS=http://+:80;http://+:8081
EXPOSE 80 8081

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY . .
WORKDIR /src/Sources/Scaffold.WebApi
RUN dotnet build Scaffold.WebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Scaffold.WebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Scaffold.WebApi.dll"]
