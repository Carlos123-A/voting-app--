FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

RUN dotnet new console -n VotingWorker

WORKDIR /app/VotingWorker

COPY Program.cs ./Program.cs

RUN dotnet add package Npgsql
RUN dotnet add package StackExchange.Redis

RUN dotnet restore

RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "VotingWorker.dll"]
