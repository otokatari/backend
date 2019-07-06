FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["OtokatariBackend.csproj", ""]
RUN dotnet restore "OtokatariBackend.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "OtokatariBackend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OtokatariBackend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OtokatariBackend.dll"]