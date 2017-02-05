FROM microsoft/dotnet:latest

ENV ASPNETCORE_ENVIRONMENT Production

COPY . /app

WORKDIR /app

RUN ["dotnet", "restore"]

RUN ["dotnet", "build"]

CMD ["dotnet", "run"]
