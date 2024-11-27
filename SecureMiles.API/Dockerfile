# Use the official .NET SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /src

# Copy the solution file and restore dependencies
COPY *.sln ./
COPY SecureMiles.API/SecureMiles.API.csproj ./SecureMiles.API/
COPY SecureMiles.Services/SecureMiles.Services.csproj ./SecureMiles.Services/
COPY SecureMiles.Repositories/SecureMiles.Repositories.csproj ./SecureMiles.Repositories/
COPY SecureMiles.Common/SecureMiles.Common.csproj ./SecureMiles.Common/
COPY SecureMiles.Models/SecureMiles.Models.csproj ./SecureMiles.Models/

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the application
COPY . ./

# Publish the application (Specify the correct output directory and project)
RUN dotnet publish SecureMiles.API -c Release -o /app/publish

# Use the .NET ASP.NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory in the container
WORKDIR /app

# Copy the published files from the build image
COPY --from=build /app/publish .

# Set the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "SecureMiles.API.dll"]
