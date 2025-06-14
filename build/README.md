# Quick Start

This directory contains build artifacts and scripts to easily run the baby-gate auth server with NATS.

> [!IMPORTANT]
> This is a proof-of-concept and should not be used in production.
> 
> The provided configuration uses example keys and credentials that should NOT be used outside of this demo:
> - NATS auth credentials: authuser/changeme
> - NKey seed: SAALGFCQB5KD4R5BDBW7IDCJ2DYPSDUD6ALZ55WM3CRV2AZZC7V6O5W6OM
> - Public key: ABBP35KOKLMR7SWBOYTEZBW3GOL74OIOUKP6KZIVE2HKQLE57SEM65LF
> 
> Generate your own keys for any real usage.

## Prerequisites

- Docker (for NATS server)
- .NET 9 SDK (for AuthServer)
- [GitHub Application](https://github.com/settings/apps/new) configured
  - as basic as you can make it, no webhooks, etc.

## Configuration

### GitHub Setup
Before running, you'll need to configure your GitHub credentials. Ideally this is done in the solution. 
But, editing the `build/authServer.json` file before running the shell script is the
quickest start. The solution authServer.json is already git-ignored.

1. Go to https://github.com/settings/apps/new
2. Create a new App
3. Set Homepage URL. I used a domain I owned, localhost should work.
4. Set Callback URL to: `https://localhost:7021/signin-github`
5. I kept `Expire user authorization tokens` checked.
6. I unchecked `Active` under **Webhook**.
7. I specified Only on this account, but not sure that it matters.
8. Edit `authServer.json` and add your GitHub credentials:
   ```json
   {
     "NatsUser": "authuser",
     "NatsPassword": "changeme", 
     "NatsSeed": "SAALGFCQB5KD4R5BDBW7IDCJ2DYPSDUD6ALZ55WM3CRV2AZZC7V6O5W6OM",
     "GitHubClient": "your-github-client-id",
     "GitHubSecret": "your-github-client-secret"
   }
   ```

## Docker on Linux

Here are the convenience scripts that I tested using Docker on Linux.

1. **Start NATS server:**
   ```bash
   cd build/
   chmod +x start-nats.sh
   ./start-nats.sh
   ```

2. **Start AuthServer:**
   ```bash
   # assumes current-directory is `build`
   # assumes authServer.json already has GitHub values
    chmod +x start-authserver.sh
   ./start-authserver.sh
   ```

3. **Access the application:**
   - AuthServer: https://localhost:7021
   - NATS Monitoring: http://localhost:8222

### Direct Execution

If you prefer to run commands directly:

### Start NATS Server
```bash
cd build/
docker run -d \
  --name nats-server \
  -p 4222:4222 \
  -p 8222:8222 \
  -p 6222:6222 \
  -v "$(pwd)/nats.conf:/nats/nats.conf:ro" \
  -v nats-data:/data \
  nats:latest \
  -c /nats/nats.conf
```

### Start AuthServer

```bash
# Copy configuration
cp authServer.json ../src/AuthServer/authServer.json
```

Update the values locally in the solution
```bash
cd ../src/AuthServer
vim authServer.json
```

Run
```bash
# assumes current-directory is `src/AuthServer`
dotnet run --launch-profile https
```

## Port Information

- **7021**: AuthServer HTTPs endpoint
- **4222**: NATS client connections
- **8222**: NATS HTTP monitoring

## Docker Management

### View NATS logs:
```bash
docker logs nats-server
```

### Stop NATS:
```bash
docker stop nats-server
```

### Remove NATS container:
```bash
docker rm nats-server
```

### Remove NATS data volume:
```bash
docker volume rm nats-data
```

## Configuration Details

### nats.conf
- Configures NATS server with JetStream enabled
- Sets up AUTH, APP, and SYS accounts
- Enables auth callout with the provided issuer key
- Uses example credentials (authuser/changeme)

### authServer.json
- Contains NATS connection credentials
- Includes the NKey seed for auth callout signing
- Requires GitHub app client ID and secret

## Troubleshooting

### NATS Connection Issues
- Ensure Docker is running
- Verify NATS container is running: `docker ps`
- Some distros require you to pass in `--network host`

### AuthServer Issues
- Ensure NATS is running and accessible
- Check that authServer.json contains valid GitHub credentials
- Verify .NET 9 SDK is installed: `dotnet --list-sdks`



