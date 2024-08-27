#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.Player.Bucket.Scheduler/Tgs.Player.Bucket.Scheduler.csproj", "Tgs.Player.Bucket.Scheduler/"]
RUN dotnet restore "Tgs.Player.Bucket.Scheduler/Tgs.Player.Bucket.Scheduler.csproj"
COPY . .
WORKDIR "/src/Tgs.Player.Bucket.Scheduler"
RUN dotnet build "Tgs.Player.Bucket.Scheduler.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.Player.Bucket.Scheduler.csproj" -c $BuildConfiguration -o /app/publish /p:UseAppHost=false

FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS base
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.Player.Bucket.Scheduler.dll"]