# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["BarberiaTurnos.csproj", "./"]
RUN dotnet restore "BarberiaTurnos.csproj"

# Copy the rest of the code and build
COPY . .
RUN dotnet build "BarberiaTurnos.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "BarberiaTurnos.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET ASP.NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (Internal to Docker, we map it in compose)
EXPOSE 8080
# Force ASP.NET Core to bind to 0.0.0.0 so we can access it from outside the container
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "BarberiaTurnos.dll"]
