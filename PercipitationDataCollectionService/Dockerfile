#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
RUN apt update && apt install -y curl
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PercipitationDataCollectionService/PrecipitationService.csproj", "PercipitationDataCollectionService/"]
RUN dotnet restore "PercipitationDataCollectionService/PrecipitationService.csproj"
COPY . .
WORKDIR "/src/PercipitationDataCollectionService"
RUN dotnet build "PrecipitationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrecipitationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrecipitationService.dll"]