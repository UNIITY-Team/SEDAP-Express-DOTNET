# Contributing

## Prerequisites

Install the .NET SDK version specified in `global.json`. To check what you have installed:

```bash
dotnet --list-sdks
```

If `dotnet` is not recognized, no SDK is installed (or you need to reopen the shell after installation).

## Building

```bash
dotnet build
```

`TreatWarningsAsErrors` is enabled globally — the build fails on any analyzer warning.

## Testing

```bash
# All tests
dotnet test

# Single test
dotnet test --filter "FullyQualifiedName~MessageSerializerContactMessageTests.MessageCanBeDeserialized"
```

Depending on your environment you may need a firewall exception for the test runner (`testhost.exe`).

## Architecture

### Source layout

```
src/SEDAPExpress.Messages/
├── Abstractions/           — enums, ISedapExpressMessage, IMessageSerializer, MessageParseException
├── Converters/             — wire-format extension classes (one per enum / logical group)
├── Serializers/            — MessageSerializer + MessageSerializer.Log.cs
├── Utilities/              — ConsoleLogger fallback
└── *Message.cs             — one sealed record per message type (15 total)

tests/SEDAPExpress.Messages.Tests/
├── *MessageTests.cs        — round-trip serialization tests (one file per message type)
└── Converters/             — unit tests for every extension class
```

### Key design decisions

- Message types are immutable `sealed record` classes implementing `ISedapExpressMessage`.
- `MessageSerializer` is the single entry point for (de)serialization.
- Enums carry no explicit integer values. All wire-format mappings live in the `Converters/` extension classes — never inline conversion logic in `MessageSerializer.cs`.
- Package versions are managed centrally in `Directory.Packages.props`. Do not specify versions in individual `.csproj` files.

For detailed architecture notes, naming conventions, and gotchas, see [CLAUDE.md](CLAUDE.md).

## Releasing

Releases are published to NuGet.org automatically by GitHub Actions when a tag is pushed.
The version is derived from the tag by [MinVer](https://github.com/adamralph/minver).

### Pre-release

```bash
git tag 1.0.0-preview.1
git push origin 1.0.0-preview.1
```

NuGet treats versions with a suffix as pre-release (hidden from search by default).
Typical progression: `1.0.0-preview.1` -> `1.0.0-rc.1` -> `1.0.0`.

### Stable release

```bash
git tag 1.0.0
git push origin 1.0.0
```

### Prerequisites

The `NUGET_API_KEY` secret must be set in **Settings -> Secrets and variables -> Actions**
(or on the `nuget` environment if environment protection is configured).
The key must be scoped to push for the `Bundeswehr.Uniity.SEDAPExpress.*` package ID glob.
