FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GHPAT
ARG VERSION
WORKDIR /src
COPY ["Texnokaktus.ProgOlymp.ContentService/Texnokaktus.ProgOlymp.ContentService.csproj", "Texnokaktus.ProgOlymp.ContentService/"]
COPY ["Texnokaktus.ProgOlymp.ContentService.DataAccess/Texnokaktus.ProgOlymp.ContentService.DataAccess.csproj", "Texnokaktus.ProgOlymp.ContentService.DataAccess/"]
RUN dotnet nuget add source --username kav128 --password $GHPAT --store-password-in-clear-text --name "GitHub Texnokaktus-Progolymp" https://nuget.pkg.github.com/Texnokaktus-ProgOlymp/index.json
RUN dotnet restore "Texnokaktus.ProgOlymp.ContentService/Texnokaktus.ProgOlymp.ContentService.csproj"
COPY . .
WORKDIR "/src/Texnokaktus.ProgOlymp.ContentService"
RUN dotnet build "./Texnokaktus.ProgOlymp.ContentService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Texnokaktus.ProgOlymp.ContentService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Texnokaktus.ProgOlymp.ContentService.dll"]
