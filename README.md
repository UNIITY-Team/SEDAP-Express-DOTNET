# SEDAP-Express-DOTNET

.NET port of [SEDAP-Express](https://github.com/UNIITY-Team/SEDAPExpress) — a library for parsing and producing SEDAP Express messages used in Bundeswehr/UNIITY systems.

## Requirements

.NET 10.0 or later.

## Usage

### Installation

Install the NuGet package:

```bash
dotnet add package <PACKAGE-NAME>
```

Or add it to your `.csproj`:

```xml
<PackageReference Include="<PACKAGE-NAME>" Version="x.y.z" />
```

### Setup

Create a `MessageSerializer`. It accepts an optional `ILogger<MessageSerializer>`; without one, log output goes to the console.

```csharp
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;

// Console logging:
MessageSerializer serializer = new();

// With a logger:
MessageSerializer serializer = new(logger);
```

With dependency injection (`IMessageSerializer` is the interface):

```csharp
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;

// MessageSerializer resolves ILogger<MessageSerializer> from the container automatically:
services.AddLogging();
services.AddSingleton<IMessageSerializer, MessageSerializer>();
```

### Deserializing messages

```csharp
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;

MessageSerializer serializer = new();

// Throws MessageParseException on failure:
ISedapExpressMessage message = serializer.Deserialize(
    "CONTACT;01;661D64C0;UNIT1;U;;;TRACK01;FALSE;52.52;13.405");

// Non-throwing overload (accepts ReadOnlySpan<char> for zero-allocation hot paths):
if (serializer.TryDeserialize(rawSpan, out ISedapExpressMessage? message))
{
    // use message
}

// Dispatch by concrete type:
switch (message)
{
    case ContactMessage contact:
        Console.WriteLine($"Contact '{contact.ContactId}' at {contact.Latitude}, {contact.Longitude}");
        break;

    case HeartbeatMessage heartbeat:
        Console.WriteLine($"Heartbeat from {heartbeat.Sender}");
        break;

    case TextMessage text:
        Console.WriteLine($"[{text.TextType}] {text.FreeText}");
        break;

    // ...
}
```

### Serializing messages

All message types are immutable `sealed record` classes. Construct one and pass it to `Serialize`:

```csharp
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;

MessageSerializer serializer = new();

ContactMessage contact = new(
    Number: 0x01,
    Time: 0x661D64C0,
    Sender: "UNIT1",
    Classification: Classification.Unclas,
    Acknowledgement: Acknowledgement.False,
    Mac: null,
    ContactId: "TRACK01",
    DeleteMode: DeleteMode.False,
    Latitude: 52.5200,
    Longitude: 13.4050,
    Altitude: null,
    RelativeXDistance: null,
    RelativeYDistance: null,
    RelativeZDistance: null,
    Speed: null,
    Course: null,
    Heading: null,
    Roll: null,
    Pitch: null,
    Width: null,
    Length: null,
    Height: null,
    Name: "Target Alpha",
    Source: null,
    Sidc: null,
    Mmsi: null,
    Icao: null,
    MultimediaData: null,
    Comment: null
);

string wire = serializer.Serialize(contact);
// → "CONTACT;01;661D64C0;UNIT1;U;;;TRACK01;FALSE;52.52;13.405;;;;;;;;;;;;;;;Target Alpha"
```

Trailing empty fields are stripped automatically on serialization.

### Wire-format extension helpers

Extension methods for converting enum values to and from their wire representations are available independently of the serializer, for custom parsing or formatting scenarios:

```csharp
using Bundeswehr.Uniity.SEDAPExpress.Messages;

// Single-char enums:
char c = Classification.Secret.ToWireChar();                               // → 'S'
bool ok = ClassificationExtensions.TryFromWireChar('S', out Classification cls); // cls = Secret

// String-mapped enums:
string s = Acknowledgement.True.ToWireString();    // → "TRUE"
string s = DeleteMode.False.ToWireString();         // → "FALSE"
string s = ContentType.Json.ToWireString();         // → "JSON"

// Byte-mapped (CommandType uses a non-sequential wire value for GenericAction = 0xFF):
byte b = CommandType.GenericAction.ToWireByte();                           // → 0xFF
bool ok = CommandTypeExtensions.TryFromWireByte(0xFF, out CommandType ct); // ct = GenericAction

// Int-mapped (all remaining enums map sequentially from 0):
int i = GraphicType.Circle.ToWireInt();                                    // → 4
bool ok = IntEnumExtensions.TryFromWireInt(4, out GraphicType gt);         // gt = Circle
```

Contact source values are char-mapped via `ContactSourceExtensions.ToWireChar` / `FromWireChar`.
Message type names are string-mapped via `MessageTypeExtensions.ToWireString` / `TryFromWireString`.

### Supported message types

| Wire name   | .NET type            | Description                            |
|-------------|----------------------|----------------------------------------|
| ACKNOWLEDGE | `AcknowledgeMessage` | Acknowledges receipt of another message |
| COMMAND     | `CommandMessage`     | Remote command (start, stop, configure, …) |
| CONTACT     | `ContactMessage`     | Tracked contact with position and identity data |
| EMISSION    | `EmissionMessage`    | Electromagnetic emission report        |
| GENERIC     | `GenericMessage`     | Freeform payload with encoding metadata |
| GRAPHIC     | `GraphicMessage`     | Graphical overlay element              |
| HEARTBEAT   | `HeartbeatMessage`   | Liveness / keepalive signal            |
| KEYEXCHANGE | `KeyexchangeMessage` | Cryptographic key exchange             |
| METEO       | `MeteoMessage`       | Meteorological data                    |
| OWNUNIT     | `OwnUnitMessage`     | Own-unit position and state report     |
| POINT       | `PointMessage`       | Named geographic point                 |
| RESEND      | `ResendMessage`      | Request to re-transmit a prior message |
| STATUS      | `StatusMessage`      | Platform technical and operational state |
| TEXT        | `TextMessage`        | Free-text message (info, warning, chat, …) |
| TIMESYNC    | `TimesyncMessage`    | Clock synchronisation                  |

### Wire format

Messages are semicolon-delimited strings. The first seven fields form the common header:

```
<MessageType>;<Number(hex)>;<Time(hex)>;<Sender>;<Classification>;<Acknowledgement>;<MAC>[;<field>...]
```

| Field            | Format                                                                  |
|------------------|-------------------------------------------------------------------------|
| `MessageType`    | Uppercase name, e.g. `CONTACT`                                          |
| `Number`         | Hex byte, `00`–`7F`                                                     |
| `Time`           | 8–16 hex digits                                                         |
| `Classification` | Single char: `P` Public · `U` Unclas · `R` Restricted · `C` Confidential · `S` Secret · `T` TopSecret · `-` None |
| `Acknowledgement`| `TRUE`, `FALSE`, or empty (treated as `FALSE`)                          |
| `MAC`            | Optional sender MAC address                                             |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).
