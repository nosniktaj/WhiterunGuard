# Use an Ubuntu base image
FROM ubuntu:24.10 AS base

RUN apt-get update && apt-get install -y python3.10 python3-pip
RUN apt-get update && apt-get install -y dotnet-sdk-9.0


WORKDIR /build

# Copy the C# project files
COPY WhiterunConfig/ ./WhiterunConfig/
COPY WhiterunGuard/ ./WhiterunGuard/
COPY WhiterunGuard.sln ./

# Restore dependencies and build the project
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Copy Python script and requirements
COPY WhiterunGuard/Python/TikTok.py ./TikTok.py
COPY WhiterunGuard/Python/requirements.txt ./requirements.txt

# Install Python dependencies
RUN pip3 install --break-system-packages -r requirements.txt

WORKDIR /app

# Clean up the build directory to remove unnecessary files
RUN rm -rf /build

# Copy C# binaries from 
ENTRYPOINT ["dotnet", "WhiterunGuardt"]	