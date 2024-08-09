#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["APIs/WebAPI/WebAPI.csproj", "APIs/WebAPI/"]
COPY ["APIs/Application/Application.csproj", "APIs/Application/"]
COPY ["APIs/Domain/Domain.csproj", "APIs/Domain/"]
COPY ["APIs/Infrastructure/Infrastructure.csproj", "APIs/Infrastructure/"]
RUN dotnet restore "./APIs/WebAPI/./WebAPI.csproj"
RUN dotnet dev-certs https --trust
COPY . .
WORKDIR "/src/APIs/WebAPI"
RUN dotnet build "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
ENTRYPOINT ["dotnet", "WebAPI.dll"]