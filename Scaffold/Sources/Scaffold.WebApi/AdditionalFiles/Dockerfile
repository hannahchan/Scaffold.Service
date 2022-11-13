FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY . .
ENTRYPOINT ["dotnet", "Scaffold.WebApi.dll"]

RUN groupadd --system app && \
    useradd --system --gid app app && \
    chown --recursive app:app /app

USER app
