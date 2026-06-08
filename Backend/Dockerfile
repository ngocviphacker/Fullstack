# Stage 1: Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Backend.csproj", "./"]
RUN dotnet restore "Backend.csproj"

# Copy all source files and compile
COPY . .
RUN dotnet build "Backend.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5003
ENV ASPNETCORE_URLS=http://+:5003
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend.dll"]
