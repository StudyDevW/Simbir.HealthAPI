#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Simbir.Health.AccountAPI/Simbir.Health.AccountAPI.csproj", "Simbir.Health.AccountAPI/"]
RUN dotnet restore "Simbir.Health.AccountAPI/Simbir.Health.AccountAPI.csproj"
COPY . .
WORKDIR "/src/Simbir.Health.AccountAPI"
RUN dotnet build "Simbir.Health.AccountAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Simbir.Health.AccountAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Simbir.Health.AccountAPI.dll"]