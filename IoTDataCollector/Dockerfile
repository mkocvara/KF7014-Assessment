#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
RUN apt update && apt install -y curl
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IoTDataCollector/IoTHumidityDataCollector.csproj", "IoTDataCollector/"]
RUN dotnet restore "IoTDataCollector/IoTHumidityDataCollector.csproj"
COPY . .
WORKDIR "/src/IoTDataCollector"
RUN dotnet build "IoTHumidityDataCollector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IoTHumidityDataCollector.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IoTHumidityDataCollector.dll"]