#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated7.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 7086

FROM mcr.microsoft.com/dotnet/runtime:7.0 as runtime7.0
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# Copy .NET Core 7.0 runtime from the 7.0 image
COPY --from=runtime7.0 /usr/share/dotnet/host /usr/share/dotnet/host
COPY --from=runtime7.0 /usr/share/dotnet/shared /usr/share/dotnet/shared
WORKDIR /src
COPY ["src/SoundForest.Api.Titles.Single/SoundForest.Api.Titles.Single.csproj", "src/SoundForest.Api.Titles.Single/"]
RUN dotnet restore "src/SoundForest.Api.Titles.Single/SoundForest.Api.Titles.Single.csproj"
COPY . .
WORKDIR "/src/src/SoundForest.Api.Titles.Single"
RUN dotnet build "SoundForest.Api.Titles.Single.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SoundForest.Api.Titles.Single.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true