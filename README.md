# SEDAP-Express-DOTNET
.NET Release of SEDAP-Express.

## Prerequisites for development
Install the .NET SDK that is specified in the current ´global.json´ if you haven't already.

If you are unsure if the proper SDK is installed, run
```
dotnet --list-sdks
```
If the `dotnet` command is not recognized, you don't have any SDK installed (or did not reopen the shell since install).

### Building the project

From project root, run:
```
dotnet build
```

## Running the tests

From project root, run:
```
dotnet test
```

Depending on your environment, you may need to create firewall exceptions for the test runner (i.e. testhost.exe).