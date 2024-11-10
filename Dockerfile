FROM ubuntu:latest
LABEL authors="mehiskasonen"

# Use the official .NET SDK as the base image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy the solution file and project files for restoring dependencies
COPY UnoRefactored.sln ./
COPY ConsoleApp/ConsoleApp.csproj ConsoleApp/
COPY ConsoleUI/ConsoleUI.csproj ConsoleUI/
COPY DAL/DAL.csproj DAL/
COPY Domain/Domain.csproj Domain/
COPY GameEngine/GameEngine.csproj GameEngine/
COPY Helpers/Helpers.csproj Helpers/
COPY MenuSystem/MenuSystem.csproj MenuSystem/
COPY WebApp/WebApp.csproj WebApp/

# Restore all dependencies
RUN dotnet restore

# Copy the entire project structure
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port that the application listens on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "WebApp.dll"]