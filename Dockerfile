# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files into the container
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET runtime image for running the application
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime

# Install native dependencies required by SkiaSharp
RUN apt-get update && apt-get install -y \
    xvfb \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxcb1 \
    libx11-xcb1 \
    libxrandr2 \
    libxi6 \
    libgl1-mesa-glx \
    libglib2.0-0 \
    libpango-1.0-0 \
    libcairo2 \
    libasound2 \
    libatk1.0-0 \
    libgdk-pixbuf2.0-0 \
    libgtk-3-0 \
    && rm -rf /var/lib/apt/lists/*

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /app/out .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "CanvasCertificateGenerator.dll"]
