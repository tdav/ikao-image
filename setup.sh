#!/bin/bash

# Exit on error
set -e

# Project Structure Variables
DIST_DIR="dist"
CONSOLE_DIR="$DIST_DIR/console"
API_DIR="$DIST_DIR/api"
CONFIG_DIR="$DIST_DIR/config"
MODELS_DIR="$CONFIG_DIR/models"
DOCKER_REPO="tdav/ofiq-api"

# Cleanup
echo "Cleaning up old build artifacts..."
rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

echo "Compiling OFIQ C++ Bridge..."
# We need to link against libofiq_lib.so.
g++ -shared -fPIC ofiq_bridge.cpp -o libofiq_bridge.so -L. -lofiq_lib -I. -Wl,-rpath,.

echo "Restoring and Building C# Console App..."
cd OFIQConsoleApp
dotnet publish -c Release -r linux-x64 --self-contained false -o "../$CONSOLE_DIR"
cd ..

echo "Restoring and Building OFIQ REST API..."
cd OFIQ.RestApi
dotnet publish -c Release -r linux-x64 --self-contained false -o "../$API_DIR"
cd ..

# Copy libraries to both application folders
echo "Distributing libraries..."
APPS=("$CONSOLE_DIR" "$API_DIR")

for APP in "${APPS[@]}"; do
    cp libofiq_lib.so "$APP/"
    cp libonnxruntime.so.1.18.1 "$APP/"
    cp libofiq_bridge.so "$APP/"
    # Create symlink for onnxruntime
    ln -sf libonnxruntime.so.1.18.1 "$APP/libonnxruntime.so"
done

# Deploy configuration and models
echo "Deploying configuration and models..."
mkdir -p "$MODELS_DIR"
cp OFIQConsoleApp/ofiq_config.jaxn "$CONFIG_DIR/"
cp -r OFIQConsoleApp/models/* "$MODELS_DIR/"

# Set executable permissions
echo "Setting executable permissions..."
chmod +x "$CONSOLE_DIR/ikao"
chmod +x "$API_DIR/OFIQ.RestApi"

# Docker Build (Optional check)
echo "Checking Docker availability..."
if command -v docker >/dev/null 2>&1 && docker info >/dev/null 2>&1; then
    echo "Building Docker image for OFIQ REST API..."
    docker build -t ofiq-rest-api:latest .
    
    echo "Tagging and Pushing image to Docker Hub ($DOCKER_REPO)..."
    docker tag ofiq-rest-api:latest "$DOCKER_REPO:latest"
    if docker push "$DOCKER_REPO:latest"; then
        echo "Image successfully pushed to Docker Hub."
        PUSH_SUCCESS=true
    else
        echo "WARNING: Docker push failed. Please check your internet connection and authentication (docker login)."
        PUSH_SUCCESS=false
    fi
    DOCKER_READY=true
else
    echo "WARNING: Docker is not installed or the daemon is not running. Skipping Docker image build."
    DOCKER_READY=false
    PUSH_SUCCESS=false
fi

echo "------------------------------------------------"
echo "Setup complete!"
echo "Artifacts are located in the '$DIST_DIR' directory."

if [ "$DOCKER_READY" = true ]; then
    echo "Local Docker image 'ofiq-rest-api:latest' has been created."
    if [ "$PUSH_SUCCESS" = true ]; then
        echo "Image is also available at: $DOCKER_REPO:latest"
    fi
    echo ""
    echo "To run the REST API in Docker:"
    echo "docker run -p 8080:8080 ofiq-rest-api:latest"
    echo ""
fi

echo "To run the Console App locally:"
echo "cd $CONSOLE_DIR && ./ikao <path_to_image>"
echo ""
echo "To run the REST API:"
echo "cd $API_DIR && ./OFIQ.RestApi"
echo "------------------------------------------------"