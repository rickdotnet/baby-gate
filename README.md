# baby-gate
A proof-of-concept auth server built with NATS.

> [!IMPORTANT]
> This is a proof-of-concept and should not be used in production.

## Getting Started

### Prerequisites

- [GitHub Application](https://github.com/settings/apps/new)
- NATS server configured for Auth Callout
  - example below

### Running

Rename `authServer.default.json` to `authServer.json` and update the configuration values as needed.

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

## Sources

This proof-of-concept builds on top of the following libraries:
- https://github.com/nats-io/nkeys.net
- https://github.com/stebet/Stebet.Nats.DistributedCache
- https://github.com/synadia-io/callout.net
- https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers