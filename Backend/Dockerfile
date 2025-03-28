# Use the official .NET SDK 9.0 as the base image for building the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the solution and project files
COPY Backend.sln ./
COPY FileApi/FileApi.csproj FileApi/

# Restore dependencies
RUN dotnet restore FileApi/FileApi.csproj

# Copy the entire source code
COPY FileApi/ FileApi/

# Set working directory inside the container
WORKDIR /app/FileApi

# Build the application
RUN dotnet publish -c Release -o out

# Use the official ASP.NET runtime image for .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set working directory inside the container
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/FileApi/out .

# Expose port 5298 for the API
EXPOSE 5298

# Set the entry point
CMD ["dotnet", "FileApi.dll"]