#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MediaOrganizer.Service/MediaOrganizer.Service.csproj", "MediaOrganizer.Service/"]
RUN dotnet restore "MediaOrganizer.Service/MediaOrganizer.Service.csproj"
COPY . .
WORKDIR "/src/MediaOrganizer.Service"
RUN dotnet build "MediaOrganizer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MediaOrganizer.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediaOrganizer.Service.dll"]