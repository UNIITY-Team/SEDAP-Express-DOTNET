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
