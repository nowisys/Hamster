FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY Hamster.sln ./
COPY src/Hamster/Hamster.csproj ./src/Hamster/
COPY src/Hamster.Plugin/Hamster.Plugin.csproj ./src/Hamster.Plugin/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release

FROM microsoft/dotnet:aspnetcore-runtime
COPY --from=build-env /app/dist/Hamster /usr/lib/hamster
ENTRYPOINT ["dotnet", "/usr/lib/hamster/Hamster.dll"]
