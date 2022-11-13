FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/Sources/Scaffold.WebApi
RUN dotnet publish Scaffold.WebApi.csproj --configuration Release --output /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Scaffold.WebApi.dll"]

RUN groupadd --system app && \
    useradd --system --gid app app && \
    chown --recursive app:app /app

USER app
