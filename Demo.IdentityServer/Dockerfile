FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Demo.IdentityServer/Demo.IdentityServer.csproj", "Demo.IdentityServer/"]
RUN dotnet restore "Demo.IdentityServer/Demo.IdentityServer.csproj"
COPY . .
WORKDIR "/src/Demo.IdentityServer"
RUN dotnet build "Demo.IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.IdentityServer.dll"]