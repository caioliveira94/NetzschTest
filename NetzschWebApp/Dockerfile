# Base Image 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# Define Word Directory
WORKDIR /app

# Copy files .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all rest and build
COPY . ./
RUN dotnet publish -c Release -o out

# Image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet", "NetzschWebApp.dll"]
