#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app
RUN apt update && apt install -y curl

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IoTHumiditySensorEmulator/IoTHumiditySensorEmulator.csproj", "IoTHumiditySensorEmulator/"]
RUN dotnet restore "IoTHumiditySensorEmulator/IoTHumiditySensorEmulator.csproj"
COPY . .
WORKDIR "/src/IoTHumiditySensorEmulator"
RUN dotnet build "IoTHumiditySensorEmulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoTHumiditySensorEmulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoTHumiditySensorEmulator.dll"]