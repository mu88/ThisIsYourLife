ARG BASE_IMAGE=mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim-arm32v7
FROM ${BASE_IMAGE} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY ["src/WebApp/WebApp.csproj", "WebApp/"]
COPY ["src/BusinessServices/BusinessServices.csproj", "BusinessServices/"]
COPY ["src/DTO/DTO.csproj", "DTO/"]
COPY ["src/Entities/Entities.csproj", "Entities/"]
COPY ["src/Persistence/Persistence.csproj", "Persistence/"]
RUN dotnet restore "WebApp/WebApp.csproj"
COPY . .
RUN dotnet build "src/WebApp/WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/WebApp/WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]