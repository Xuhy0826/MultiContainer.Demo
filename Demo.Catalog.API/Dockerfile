#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Demo.Catalog.API/Demo.Catalog.API.csproj", "Demo.Catalog.API/"]
RUN dotnet restore "Demo.Catalog.API/Demo.Catalog.API.csproj"
COPY . .
WORKDIR "/src/Demo.Catalog.API"
RUN dotnet build "Demo.Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Catalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Catalog.API.dll"]