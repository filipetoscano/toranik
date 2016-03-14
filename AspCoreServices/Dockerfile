
# https://github.com/aspnet/aspnet-docker
FROM microsoft/aspnet:1.0.0-rc1-update1-coreclr
MAINTAINER Filipe Toscano <filipe.toscano@gmail.com>

# Private NuGet server
RUN [ "mkdir", "-p", "/root/.config/NuGet" ]
COPY NuGet.config /root/.config/NuGet/

# Application 
COPY /WebApplication /app
WORKDIR /app
RUN [ "dnu", "restore" ]

EXPOSE 5001
ENTRYPOINT [ "dnx", "-p", "project.json", "web" ]

# eof