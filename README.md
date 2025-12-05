# das-payments-v2-providerpayments-republish-function

This tool is designed to retrieve a JSON document from Azure Blob Storage, parse the serialized Provider Payments messages from the document, and then re-submit those messages to the Provider Payments service bus queue.

To set up on your local machine:

Create a file called local.settings.json with the following content:

```
{
  "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "ApplicationInsightsInstrumentationKey": "<< your development Application Insights key >>",
        "FUNCTIONS_INPROC_NET8_ENABLED": "1",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "ServiceBusConnectionString": "<< your Payments V2 service bus namespace connection string - namespace will start with 'das-pv2-dev-' >>",
        "EndpointName": "sfa-das-payments-providerpayments-republish",
	"DestinationEndpointName": "sfa-das-payments-providerpayments",
	"FailedMessagesQueue": "sfa-das-payments-providerpayments-republish-errors",
        "NServiceBusLicense": "",
        "StorageConnectionString": "UseDevelopmentStorage=true",
        "ImmediateMessageRetries": "1",
        "DelayedMessageRetries": "3",
        "DelayedMessageRetryDelay": "00:00:10"
    }
}
```

To run locally:

Create a new Blob Container in your local Azurite / Azure Storage Emulator instance.
Upload a test JSON file to the container.

Using Postman or your API test tool of choice, create a POST request to URL:

http://localhost:7132/api/HttpTriggerResubmitCompletionPaymentsMessages

With the JSON payload:

```
{
"BlobStorageContainerName": "<< container name >>",
"FileName": "<< filename of test JSON file >>"
}
```

## Technologies

.NET8
Azure Functions
Azure Blob Storage
Azure Service Bus