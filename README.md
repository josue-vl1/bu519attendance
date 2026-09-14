# BU519 Attendance

A Blazor Server application for managing class rosters and attendance in Azure Blob Storage.

## Requirements

- .NET 10 SDK
- An Azure identity with access to the `studentinfo` storage account
- Azure App Service Authentication configured to provide the `X-MS-CLIENT-PRINCIPAL-NAME` header

## Configuration

Configure these environment variables or equivalent Azure App Service settings:

| Setting | Purpose |
| --- | --- |
| `CLASSFOLDER` | Blob container and class identifier; defaults to `Classes` |
| `CLASSINFO_JSON` | Class schedule, date range, and instructor list serialized as JSON |
| `OPENAI_ENDPOINT` | Azure OpenAI endpoint used by bulk import |
| `OPENAI_DEPLOYMENT` | Azure OpenAI deployment used by bulk import |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Optional Application Insights connection string |

`DefaultAzureCredential` is used for Azure Storage and Azure OpenAI. For local development, sign in with a supported developer credential. In Azure, assign the web app a managed identity with the required data-plane roles.

## Run locally

```bash
dotnet restore
dotnet run --launch-profile https
```

The deployment target must have the .NET 10 runtime available. The GitHub Actions workflow builds and publishes with the .NET 10 SDK.
