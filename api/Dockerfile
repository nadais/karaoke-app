FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Karaoke.Api/Karaoke.Api.csproj", "Karaoke.Api/"]
RUN dotnet restore "Karaoke.Api/Karaoke.Api.csproj"
COPY . .
WORKDIR "/src/Karaoke.Api"
RUN dotnet build "Karaoke.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Karaoke.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "Karaoke.Api.dll"]
