# MzansiBuilds Backend - ASP.NET Core WebAPI

## Setup & Installation
1. Install dependencies:
   `dotnet restore`
2. Run the application:
   `dotnet run --project Backend`
3. Access the API documentation at: `https://localhost:7203/swagger`

## Managing Application Secrets
To ensure that unique variables in the backend are not exposed. This project makes use of the secrets.json embedded in your local application. Update the secrets.json by right clicking the Backend project in Visual Studio and navigating to ##Manage User Secrets## and change the file to follow the following stucture with your relevant variables.
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server={YourLocalServer};Database={DatabaseName};Trust_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Testing 
1. Run API unit tests
   `dotnet test --no-build`

## UML Class Architecture
<img width="4096" height="2555" alt="image" src="https://github.com/user-attachments/assets/8ddbf7c7-407f-413a-98ba-46d387ed4fe8" />
The UML class diagram above represents the class structure that will be used as to communicate with the data layer of the application. 
