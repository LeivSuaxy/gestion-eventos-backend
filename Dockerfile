FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["event-horizon-backend/event-horizon-backend.csproj", "event-horizon-backend/"]
RUN dotnet restore "event-horizon-backend/event-horizon-backend.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/event-horizon-backend"
RUN dotnet build "event-horizon-backend.csproj" -c Release -o /app/build
RUN dotnet publish "event-horizon-backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway automatically assigns a PORT - make sure your app reads from it
ENV ASPNETCORE_URLS=http://+:$PORT
ENV Global__Production=true
ENV Redis__Enabled=false

ENTRYPOINT ["sh", "-c","dotnet", "/event-horizon-backend/event-horizon-backend.dll"]
