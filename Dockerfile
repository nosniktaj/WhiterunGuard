# Use an Ubuntu base image
FROM ubuntu:24.10 AS base

# Install DotNet SDK
RUN apt-get update && apt-get install -y dotnet-sdk-9.0

# Install Python and pip
RUN apt-get update && apt-get install -y python3.10 python3-pip

# Copy the C# project files
WORKDIR /build
COPY WhiterunConfig/ ./WhiterunConfig/
COPY WhiterunGuard/ ./WhiterunGuard/
COPY WhiterunGuard.sln ./

# Restore dependencies and build the project
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Install Python dependencies
RUN pip3 install --break-system-packages -r WhiterunGuard/Python/requirements.txt

WORKDIR /app

# Clean up the build directory to remove unnecessary files
RUN rm -rf /build

# Copy C# binaries from 
ENTRYPOINT ["dotnet", "WhiterunGuard.dll"]	