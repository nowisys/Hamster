FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY Hamster.sln ./
COPY src/Hamster/Hamster.csproj ./src/Hamster/
COPY src/Hamster.Plugin/Hamster.Plugin.csproj ./src/Hamster.Plugin/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o dist

FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /app/dist /usr/lib/hamster
ENTRYPOINT ["dotnet", "/usr/lib/hamster/Hamster.dll"]
