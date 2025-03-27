# Exchange Rate API

## Overview
ExchangeRateApi is a ASP.NET Core Web API for managing exchange rates between different currencies.
The project includes database interactions, message queue publishing, and CRUD operations for exchange rates.

## Prerequisites
- .NET 9 SDK
- SQL Server
- RabbitMQ

## Setup Instructions

### 1. Clone the Repository
`git clone git clone https://github.com/IchiroKyou/Vfx_Tech_Challenge.git`

### 2. Restore nuget packages
`dotnet restore`

### 3. Set Environment Variables in Your System
Set the following environment variables in your system:
- `ExchangeApiDbConnection`
- `AlphavantageApiKey`
- `RabbitMqHostname`
#### On Windows (PowerShell with admin rights)
```
[Environment]::SetEnvironmentVariable("ExchangeApiDbConnection", "YOUR_SQL_SERVER_DB_CONNECTION_STRING", "Machine")
[Environment]::SetEnvironmentVariable("AlphavantageApiKey", "YOUR_ALPHAVANTAGE_API_KEY", "Machine")
[Environment]::SetEnvironmentVariable("RabbitMqHostname", "YOUR_RABBITMQ_HOSTNAME", "Machine")
```

### 4. Apply Database Migrations
Install Entity Framework Core tools globally if necessary:
`dotnet tool install --global dotnet-ef`

Run the following command to apply the database migrations:
`dotnet ef database update`

### 5. Build and run
`dotnet build`
`dotnet run`

## How to use
Either launch the application and then use some tool like <em>Postman</em> or <em>Insomnia</em>, or launch the application and access the URL followed by <em>/swagger</em>.
For example: `https://localhost:50189/swagger`
If you are using Visual Studio, you can also update your <em>launchSettings.json</em> to include a <em>launchUrl</em> with <em>swagger</em>:
```
{
  "profiles": {
    "ExchangeRateApi": {
      "commandName": "Project",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:50189;http://localhost:50190",
      "launchUrl": "swagger"
    }
  }
}
```

## Additional Information
- Steps [1](#1-clone-the-repository), [2](#2-restore-nuget-packages), [4](#4-apply-database-migrations), and [5](#5-build-and-run) can be simplified if using Visual Studio.
- Unit tests can be run in Visual Studio.
- Ensure that SQL Server and RabbitMQ are running and accessible with the provided connection strings.
- The application uses Entity Framework Core for database interactions and RabbitMQ for message queue publishing.
