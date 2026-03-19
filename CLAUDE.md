# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run all tests with coverage (Cobertura XML output)
dotnet test --settings coverage.runsettings

# Run a single test
dotnet test --filter "FullyQualifiedName~MessageSerializerAcknowledgeMessageTests.MessageCanBeDeserialized"
```

`TreatWarningsAsErrors` is enabled globally — the build will fail on any analyzer warning. CI enforces 90 % line coverage via `coverage.runsettings` and coverlet.

A [dev container](.devcontainer/devcontainer.json) is available for VS Code / Codespaces (.NET 10 SDK, C# Dev Kit).

## Architecture

This is a .NET port of the Java [SEDAP-Express](https://github.com/UNIITY-Team/SEDAPExpress) library. The canonical Java source lives at `~/repos/SEDAP-Express/SEDAPExpress/src/main/java/de/bundeswehr/uniity/sedapexpress/messages/` and is the authoritative reference for message field definitions, validation rules, and wire format.

### Wire format

Messages are semicolon-delimited strings. The first 7 fields are the common header, followed by message-type-specific fields:

```
<MessageType>;<Number(hex)>;<Time(hex)>;<Sender>;<Classification(char)>;<Acknowledgement(TRUE/FALSE/empty)>;<MAC>[;<field>...]
```

- **Number** and message-type-specific number fields: hex byte, range `00`–`7F`
- **Time**: 8–16 hex digits
- **Classification**: single char — `P`=Public, `U`=Unclas, `R`=Restricted, `C`=Confidential, `S`=Secret, `T`=TopSecret, `-`=None
- **Acknowledgement**: `TRUE`, `FALSE`, or empty (= FALSE)
- Trailing empty semicolon-separated fields are stripped on serialization

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
- `MessageSerializer` is the single entry point for (de)serialization. It accepts an optional `ILogger<MessageSerializer>`; if none is provided it falls back to `ConsoleLogger`.
- Compile-time log messages are defined in `MessageSerializer.Log.cs` using `[LoggerMessage]`.
- Package versions are managed centrally in `Directory.Packages.props` (Central Package Management). Do not specify versions in individual `.csproj` files.
- `Directory.Build.props` applies `LangVersion=latest`, `Nullable=enable`, `TreatWarningsAsErrors=true`, and `AnalysisMode=AllEnabledByDefault` to all projects. Test projects suppress CS1591 (missing XML doc comments).
- **Enums carry no explicit integer values.** All wire-format mappings — including non-sequential ones like `CommandType` (where `GenericAction` is `0xFF` on the wire) and `ContactSource` (char values) — live exclusively in the `Converters/` extension classes. Never add explicit enum values or inline conversion logic to `MessageSerializer.cs`.
- **`BigInteger` hex parsing**: When parsing hex strings into `BigInteger`, prepend `"00"` before calling `BigInteger.TryParse(..., NumberStyles.HexNumber, ...)` to ensure the result is always positive (Java `new BigInteger(hex, 16)` is unsigned; .NET's parser treats a leading nibble ≥ 8 as a sign bit).

### Converters/ — wire-format extension classes

Each class is `public static` in the `Bundeswehr.Uniity.SEDAPExpress.Messages` namespace (same as the message types, one level up from `Abstractions`). The folder sits under `Converters/` purely for organisation; the `IDE0130` namespace-mismatch diagnostic is set to `:suggestion` in `.editorconfig` so it does not cause a build error.

| Class                       | Enum(s)                                                                                                                                                              | Wire format                                                         |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| `ClassificationExtensions`  | `Classification`                                                                                                                                                     | `char` (`'P'`, `'U'`, …, `'-'`)                                     |
| `AcknowledgementExtensions` | `Acknowledgement`                                                                                                                                                    | `"TRUE"` or `""`                                                    |
| `DeleteModeExtensions`      | `DeleteMode`                                                                                                                                                         | `"TRUE"` or `"FALSE"`                                               |
| `DataEncodingExtensions`    | `DataEncoding`                                                                                                                                                       | `"BASE64"` or `"NONE"`                                              |
| `ContentTypeExtensions`     | `ContentType`                                                                                                                                                        | `"SEDAP"`, `"ASCII"`, `"NMEA"`, `"XML"`, `"JSON"`, `"BINARY"`       |
| `MessageTypeExtensions`     | `MessageType`                                                                                                                                                        | Uppercase name, e.g. `"CONTACT"`                                    |
| `ContactSourceExtensions`   | `ContactSource`                                                                                                                                                      | `char` (`'R'`, `'A'`, `' '`, …) — non-sequential                    |
| `CommandTypeExtensions`     | `CommandType`                                                                                                                                                        | `byte` (`0x00`–`0x09`, `0xFF` for `GenericAction`) — non-sequential |
| `IntEnumExtensions`         | `AlgorithmType`, `CommandFlagType`, `CommandState`, `EmissionFunction`, `FreqAgility`, `GraphicType`, `OperationalState`, `PrfAgility`, `TechnicalState`, `TextType` | `int` (sequential from 0)                                           |

### Naming conventions (from `.editorconfig`)

- Private instance fields: `_camelCase`
- Private static fields: `s_camelCase`
- No `var` — use explicit types everywhere
- **No underscores in member names** (CA1707 is a build error). This applies to test methods too — use `PascalCase` without the common `Method_Scenario_Expected` pattern. E.g. `ToWireCharReturnsExpectedChar`, not `ToWireChar_ReturnsExpectedChar`.
