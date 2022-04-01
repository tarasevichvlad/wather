FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Weather.Tools.Parser/Weather.Tools.Parser.csproj", "Weather.Tools.Parser/"]
RUN dotnet restore "Weather.Tools.Parser/Weather.Tools.Parser.csproj"
COPY . .
WORKDIR "/src/Weather.Tools.Parser"
RUN dotnet build "Weather.Tools.Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Weather.Tools.Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Weather.Tools.Parser.dll"]
