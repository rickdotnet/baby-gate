#!/bin/bash

# Run the AuthServer application
# This script should be run from the build/ directory
# Make sure to configure authServer.json with your GitHub OAuth app credentials

echo "Starting AuthServer application..."
echo "Using configuration: $(pwd)/authServer.json"
echo "AuthServer will be available at:"
echo "https://localhost:7021, http://localhost:5108"
echo ""

# Copy the configuration to the AuthServer directory
cp "$(pwd)/authServer.json" "../src/AuthServer/authServer.json"

# Navigate to the AuthServer project directory and run
cd ../src/AuthServer

echo "Building and running AuthServer..."
dotnet run --launch-profile https

echo "AuthServer has stopped."