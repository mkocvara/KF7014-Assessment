#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
RUN apt update && apt install -y curl
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TemperatureAPI.Sensor/TemperatureAPI.Sensor.csproj", "TemperatureAPI.Sensor/"]
RUN dotnet restore "TemperatureAPI.Sensor/TemperatureAPI.Sensor.csproj"
COPY . .
WORKDIR "/src/TemperatureAPI.Sensor"
RUN dotnet build "TemperatureAPI.Sensor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TemperatureAPI.Sensor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TemperatureAPI.Sensor.dll"]