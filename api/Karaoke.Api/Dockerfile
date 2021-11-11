FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Karaoke-catalog-server/Karaoke-catalog-server.csproj", "Karaoke-catalog-server/"]
RUN dotnet restore "Karaoke-catalog-server/Karaoke-catalog-server.csproj"
COPY . .
WORKDIR "/src/Karaoke-catalog-server"
RUN dotnet build "Karaoke-catalog-server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Karaoke-catalog-server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Karaoke-catalog-server.dll"]
