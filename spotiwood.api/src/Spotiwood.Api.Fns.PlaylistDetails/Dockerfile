#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated6.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 7073

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Spotiwood.Api.Fns.PlaylistDetails/Spotiwood.Api.Fns.PlaylistDetails.csproj", "src/Spotiwood.Api.Fns.PlaylistDetails/"]
COPY ["src/Spotiwood.Api.Playlists/Spotiwood.Api.Playlists.csproj", "src/Spotiwood.Api.Playlists/"]
COPY ["src/Spotiwood.Framework/Spotiwood.Framework.csproj", "src/Spotiwood.Framework/"]
COPY ["src/Spotiwood.Framework.Authentication/Spotiwood.Framework.Authentication.csproj", "src/Spotiwood.Framework.Authentication/"]
RUN dotnet restore "src/Spotiwood.Api.Fns.PlaylistDetails/Spotiwood.Api.Fns.PlaylistDetails.csproj"
COPY . .
WORKDIR "/src/src/Spotiwood.Api.Fns.PlaylistDetails"
RUN dotnet build "Spotiwood.Api.Fns.PlaylistDetails.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Spotiwood.Api.Fns.PlaylistDetails.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true