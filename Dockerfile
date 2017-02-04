FROM microsoft/dotnet:latest

COPY . /app

WORKDIR /app

RUN ["dotnet", "restore"]

RUN ["dotnet", "build"]

ENV ["ASPNETCORE_ENVIRONMENT", "Development"]

CMD ["dotnet", "run"]
