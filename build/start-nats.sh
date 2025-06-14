#!/bin/bash

# Start NATS server with Docker using the configuration in this directory
# This script should be run from the build/ directory

echo "Starting NATS server with Docker..."
echo "Using configuration: $(pwd)/nats.conf"
echo "NATS monitoring available at: http://localhost:8222"
echo ""

docker run -d \
  --name baby-gate \
  -p 4222:4222 \
  -p 8222:8222 \
  -p 6222:6222 \
  -v "$(pwd)/nats.conf:/nats/nats.conf" \
  -v nats-data:/data \
  nats:latest \
  -c /nats/nats.conf

echo "NATS server container started."
echo "To view logs: docker logs baby-gate"
echo "To stop: docker stop baby-gate"
echo "To remove: docker rm baby-gate"