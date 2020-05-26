#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Demo.User.API/Demo.User.API.csproj", "Demo.User.API/"]
RUN dotnet restore "Demo.User.API/Demo.User.API.csproj"
COPY . .
WORKDIR "/src/Demo.User.API"
RUN dotnet build "Demo.User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.User.API.dll"]