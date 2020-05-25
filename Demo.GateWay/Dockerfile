FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Demo.GateWay/Demo.GateWay.csproj", "Demo.GateWay/"]
RUN dotnet restore "Demo.GateWay/Demo.GateWay.csproj"
COPY . .
WORKDIR "/src/Demo.GateWay"
RUN dotnet build "Demo.GateWay.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.GateWay.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.GateWay.dll"]