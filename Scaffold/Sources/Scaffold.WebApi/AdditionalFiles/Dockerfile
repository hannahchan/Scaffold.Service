FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_HEALTHCHECKPORT=8081
ENV ASPNETCORE_METRICSPORT=8081
ENV ASPNETCORE_URLS=http://+:80;http://+:8081
EXPOSE 80 8081

COPY . .
ENTRYPOINT ["dotnet", "Scaffold.WebApi.dll"]

RUN groupadd --system app && \
    useradd --system --gid app app && \
    chown --recursive app:app /app

USER app
