# DebiCheck-Collections

## Prerequisites
- .NET 8 SDK

## Setup

1. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
2. Build the project:
   ```bash
   dotnet build
   ```

## Configuration

Create an `appsettings.json` file inside the `DCCollectionsRequest` directory. It should provide the database connection and SQL queries used by the application. Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CollectionsDb;User Id=username;Password=password;"
  },
  "SqlQueries": {
    "Collections": "SELECT * FROM Collections WHERE Processed = 0;",
    "CreditorDefaults": "SELECT * FROM CreditorDefaults WHERE CreditorId = @CreditorId;"
  }
}
```

## Running

The executable expects a fixed width file named `RM-Collections.txt` in the `DCCollectionsRequest` directory. The sample file is included and is copied to the output folder during the build.

Run the application from the repository root with:

```bash
 dotnet run --project DCCollectionsRequest/DCCollectionsRequest.csproj
```

This will parse `RM-Collections.txt` and output information about the file.
