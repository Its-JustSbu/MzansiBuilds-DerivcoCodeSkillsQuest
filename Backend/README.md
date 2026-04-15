# MzansiBuilds Backend - ASP.NET Core WebAPI

## Setup & Installation
1. Install dependencies:
   `dotnet restore`
2. Run the application:
   `dotnet run --project Backend`
3. Access the API documentation at: `https://localhost:7203/swagger`

## Testing 
1. Run API unit tests
   `dotnet test --no-build`

## Database Configuration
To connect the application to your database, you must add your connection string to the appsettings.json file located in the root of the project. This allows the application to retrieve environment-specific credentials at runtime.
1. Open appsettings.json.
2. Add a ConnectionStrings section (if it doesn't already exist).
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
3. Provide a key name (e.g., "DefaultConnection") and your actual connection string value.

## UML Class Architecture
<img width="4096" height="2251" alt="image" src="https://github.com/user-attachments/assets/e3703495-4146-45b2-b6f1-dc348861455c" />
The UML class diagram above represents the class structure that will be used as to communicate with the data layer of the application. 
