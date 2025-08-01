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

Create an `appsettings.json` file inside the `PAMC.DatabaseConnection` directory. It provides the database connection and the path to the SQL query files used by the applications. Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CollectionsDb;User Id=username;Password=password;"
  },
  "SqlQueriesPath": "SqlQueries",
  "EftRejectionCodes": {
    "000": "SUCCESSFUL",
    "002": "NOT PROVIDED FOR"
  }
}
```

Create a folder named `SqlQueries` next to `appsettings.json` in the `PAMC.DatabaseConnection` directory and place the queries inside `Collections.sql` and `CreditorDefaults.sql`. These files support normal line breaks for readability.

## Running

The executable expects a fixed width file following the pattern `ZRcode.AUL.DATA.DATE` in the `DCCollectionsRequest` directory. A sample named `ZR07675.AUL.DATA.250529.122006` is included and copied to the output folder during the build.

Run the application from the repository root with:

```bash
 dotnet run --project DCCollectionsRequest/DCCollectionsRequest.csproj
```

This will parse `ZR07675.AUL.DATA.250529.122006` and output information about the file.

To generate a collection file for a specific deduction day, provide the day as
the first command line argument and choose option `2` when prompted:

```bash
 dotnet run --project DCCollectionsRequest/DCCollectionsRequest.csproj 15
```
