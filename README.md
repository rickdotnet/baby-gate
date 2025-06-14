# baby-gate
A proof-of-concept auth server built with NATS.

> [!IMPORTANT]
> This is a proof-of-concept and should not be used in production.

## Getting Started

### Prerequisites

- [GitHub Application](https://github.com/settings/apps/new)
- NATS server configured for [Auth Callout](https://docs.nats.io/running-a-nats-service/configuration/securing_nats/auth_callout)
  - example below

### Quick Start

To quickly run the auth server with NATS, you can use the instructions provided in the [build/README.md](build/README.md) file.

### Running

1. Open the solution and copy `authServer.default.json` to `authServer.json`.
2. Add your GitHub credentials.
3. Ensure NATS is running and the configurations match
4. Run the `AuthServer` project.


## Components

### Jwt and PAT Minting

The demo creates a NATS flavored JWT and optionally stores a shorter PAT value in a [Stebet.Nats.DistributedCache](https://github.com/stebet/Stebet.Nats.DistributedCache) backed cache. 
When a client connects, values that are passed in the `Token` field will be verified directly. Otherwise, it will check the `Password` field for the PAT and look the JWT in the cache.

### Auth Callout

#### Key Pair

You'll need to generate at least one key pair to use auth callout. This uses the [nkeys.net](https://github.com/nats-io/nkeys.net) library.

```csharp
var pair = KeyPair.CreatePair(PrefixByte.Account);
var seed = pair.GetSeed();
var publicKey = pair.GetPublicKey();

Console.WriteLine($"Seed: {seed}");
Console.WriteLine($"NKey: {publicKey}");

// Seed: SAALGFCQB5KD4R5BDBW7IDCJ2DYPSDUD6ALZ55WM3CRV2AZZC7V6O5W6OM
//    - used in authServer.json
// NKey: ABBP35KOKLMR7SWBOYTEZBW3GOL74OIOUKP6KZIVE2HKQLE57SEM65LF
//    - used in nats.conf

```

authServer.json
```json
{
  "NatsUser": "authuser",
  "NatsPassword": "changeme",
  "NatsSeed": "SAALGFCQB5KD4R5BDBW7IDCJ2DYPSDUD6ALZ55WM3CRV2AZZC7V6O5W6OM",
  "GitHubClient": "",
  "GitHubSecret": ""
}
```

nats.conf
```config
http_port: 8222

jetstream {
    store_dir: /data
    max_mem: 1G
    max_file: 30G
}

accounts {
  AUTH: {
    jetstream: enabled
    users: [
      { user: "authuser", pass: "changeme" }
    ]
  }

  APP: {
    jetstream: enabled
  }
  SYS: {
  }
}

system_account: SYS

authorization {
  auth_callout {
    issuer: ABBP35KOKLMR7SWBOYTEZBW3GOL74OIOUKP6KZIVE2HKQLE57SEM65LF
    auth_users: [ "authuser" ]
    account: AUTH
  }
}
```

NATS CLI context
```json
{
  "url": "nats://localhost:4222",
  "user": "required-but-not-used",
  "password": "pat-generated-by-auth-server"
}

```

## Sources

This proof-of-concept builds on top of the following libraries:
- https://github.com/nats-io/nkeys.net
- https://github.com/stebet/Stebet.Nats.DistributedCache
- https://github.com/synadia-io/callout.net
- https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
