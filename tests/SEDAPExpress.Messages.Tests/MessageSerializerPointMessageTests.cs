using System;
using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerPointMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    // The small PNG embedded in POINTTest.java (base64-encoded multimedia field)
    private const string MultimediaBase64 =
        "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAACXBIWXMAAC4jAAAuIwF4pT92AAABcElEQVRYw+" +
        "1WsYrCUBCcyLUiNinEL1AUxNpKgoLYiqWg+AvmE/wAS0sLsbERC4UgFikEowhqqY0WQRQEsUr2ii0OTzkuuUvC" +
        "HVkI+3b3EYaZHXgCERE8jAA8Dh+AD+APA+h0AEEAtlsPAJgm0O3yeTb7GQVkJ3Y7IoBIlokkicgwyG7YY0DTAF" +
        "EE8nlgPAb2e5cl6PeBWg2Ix7mez12U4Hhk+gcDrksl/kzTJQmWS86JBOdiEej1gMPBJQmGQ0CSgGiU62TyEZij" +
        "EpxOTH+7/dG73YiCQaJ63ZYE1gAoCgNYLB77ssx9XbcM4M0SXYrCOZV6PV+tgGzW0i+Fb7+IrlcgFAIqFaBQeJ" +
        "5Vq0CjATSbDu2AqjLNo9HzzDCIMhmeXy4O2XA65RyLvfBSACiX+bxeO2DD+x1otYBcDohEXt9JpzlPJg5IoGnP" +
        "9vsc5zPfEUW25q9KoKpfbz8AhMO8iLoObDYOuMB/kvkAfAD/FcA7SSgc2Vo2QpYAAAAASUVORK5CYII=";

    public MessageSerializerPointMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void FullMessageWithMultimediaDataCanBeDeserialized()
    {
        // Comment decoded from base64 "UmVuZGV6dm91cw==" = "Rendezvous"
        string input =
            "POINT;66;1B351C87;59CE;U;TRUE;FFAA327B;1000;;43.21;-111.22;10011.0;1.0;2.0;3.0;200.0;275.0;10.0;20.0;30.0;33.0;22.0;11.0;Rendezvous;gfgpgpoz-------;"
            + MultimediaBase64 + ";UmVuZGV6dm91cw==";
        ISedapExpressMessage actualBase = _sut.Deserialize(input);
        PointMessage actual = Assert.IsType<PointMessage>(actualBase);
        PointMessage expected = new PointMessage(
            Number: 0x66,
            Time: 0x1B351C87,
            Sender: "59CE",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.True,
            Mac: "FFAA327B",
            ContactId: "1000",
            DeleteMode: DeleteMode.False,
            Latitude: 43.21,
            Longitude: -111.22,
            Altitude: 10011.0,
            RelativeXDistance: 1.0,
            RelativeYDistance: 2.0,
            RelativeZDistance: 3.0,
            Speed: 200.0,
            Course: 275.0,
            Heading: 10.0,
            Roll: 20.0,
            Pitch: 30.0,
            Width: 33.0,
            Length: 22.0,
            Height: 11.0,
            Name: "Rendezvous",
            Sidc: "gfgpgpoz-------",
            MultimediaData: Convert.FromBase64String(MultimediaBase64),
            Comment: "Rendezvous"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithCommentAndNoMultimediaCanBeDeserialized()
    {
        // Comment decoded from base64 "VGFyZ2V0IHBvaW50" = "Target point"
        string input = "POINT;5E;661D4410;66A3;R;;;100;FALSE;53.32;8.11;0;;;;120;275;;;;;;;Target Alpha;gffppt---------;;VGFyZ2V0IHBvaW50";
        ISedapExpressMessage actualBase = _sut.Deserialize(input);
        PointMessage actual = Assert.IsType<PointMessage>(actualBase);
        PointMessage expected = new PointMessage(
            Number: 0x5E,
            Time: 0x661D4410,
            Sender: "66A3",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "100",
            DeleteMode: DeleteMode.False,
            Latitude: 53.32,
            Longitude: 8.11,
            Altitude: 0.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: 120.0,
            Course: 275.0,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: "Target Alpha",
            Sidc: "gffppt---------",
            MultimediaData: null,
            Comment: "Target point"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithCommentAndNoMultimediaCanBeSerialized()
    {
        PointMessage message = new PointMessage(
            Number: 0x5E,
            Time: 0x661D4410,
            Sender: "66A3",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "100",
            DeleteMode: DeleteMode.False,
            Latitude: 53.32,
            Longitude: 8.11,
            Altitude: 0.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: 120.0,
            Course: 275.0,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: "Target Alpha",
            Sidc: "gffppt---------",
            MultimediaData: null,
            Comment: "Target point"
        );
        string actual = _sut.Serialize(message);
        string expected = "POINT;5E;661D4410;66A3;R;;;100;FALSE;53.32;8.11;0;;;;120;275;;;;;;;Target Alpha;gffppt---------;;VGFyZ2V0IHBvaW50";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MessageWithMinimalFieldsCanBeDeserialized()
    {
        // Comment decoded from base64 "UG9zcyBOZXRoZXJsYW5kcw==" = "Poss Netherlands"
        string input = "POINT;5F;661D5420;83C5;U;;;101;FALSE;36.32;12.11;2000;;;;44;;;;;;;;Unknown;;;UG9zcyBOZXRoZXJsYW5kcw==";
        ISedapExpressMessage actualBase = _sut.Deserialize(input);
        PointMessage actual = Assert.IsType<PointMessage>(actualBase);
        PointMessage expected = new PointMessage(
            Number: 0x5F,
            Time: 0x661D5420,
            Sender: "83C5",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "101",
            DeleteMode: DeleteMode.False,
            Latitude: 36.32,
            Longitude: 12.11,
            Altitude: 2000.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: 44.0,
            Course: null,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: "Unknown",
            Sidc: null,
            MultimediaData: null,
            Comment: "Poss Netherlands"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithMinimalFieldsCanBeSerialized()
    {
        PointMessage message = new PointMessage(
            Number: 0x5F,
            Time: 0x661D5420,
            Sender: "83C5",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "101",
            DeleteMode: DeleteMode.False,
            Latitude: 36.32,
            Longitude: 12.11,
            Altitude: 2000.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: 44.0,
            Course: null,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: "Unknown",
            Sidc: null,
            MultimediaData: null,
            Comment: "Poss Netherlands"
        );
        string actual = _sut.Serialize(message);
        string expected = "POINT;5F;661D5420;83C5;U;;;101;FALSE;36.32;12.11;2000;;;;44;;;;;;;;Unknown;;;UG9zcyBOZXRoZXJsYW5kcw==";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MinimalDeleteMessageCanBeDeserialized()
    {
        string input = "POINT;60;54742310;4371;S;TRUE;;102;TRUE;53.32;8.11";
        ISedapExpressMessage actualBase = _sut.Deserialize(input);
        PointMessage actual = Assert.IsType<PointMessage>(actualBase);
        PointMessage expected = new PointMessage(
            Number: 0x60,
            Time: 0x54742310,
            Sender: "4371",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            ContactId: "102",
            DeleteMode: DeleteMode.True,
            Latitude: 53.32,
            Longitude: 8.11,
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
            Name: null,
            Sidc: null,
            MultimediaData: null,
            Comment: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MinimalDeleteMessageCanBeSerialized()
    {
        PointMessage message = new PointMessage(
            Number: 0x60,
            Time: 0x54742310,
            Sender: "4371",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            ContactId: "102",
            DeleteMode: DeleteMode.True,
            Latitude: 53.32,
            Longitude: 8.11,
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
            Name: null,
            Sidc: null,
            MultimediaData: null,
            Comment: null
        );
        string actual = _sut.Serialize(message);
        string expected = "POINT;60;54742310;4371;S;TRUE;;102;TRUE;53.32;8.11";
        Assert.Equal(expected, actual);
    }
}
