# AzureAiDocumentIntelligenceMay2024

This project demonstrates the usage of the Azure AI Document Intelligence service to analyze documents, extract key-value pairs, and process specific fields from images and PDFs. 
The application is built with .NET and uses dependency injection for configuration management.

## Table of Contents

- [Overview](#overview)
- [Setup](#setup)
- [Usage](#usage)
- [Configuration](#configuration)
- [Dependencies](#dependencies)
- [Contributing](#contributing)
- [License](#license)

## Overview

The `AzureAiDocumentIntelligenceMay2024` project uses the Azure AI Document Intelligence service to read and analyze values from various document formats such as images and PDFs. 
It extracts valuable information, such as vendor names, customer names, invoice totals, and more, and outputs the extracted data to the console.

References: 
- MS Learn [Azure AI Document Intelligence Overview](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/overview?view=doc-intel-4.0.0)
- Azure AI [Document Intelligence Studio](https://documentintelligence.ai.azure.com/)
- MS Learn Azure Form Recognizer client library v4.1.0 [Sample](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/ai.formrecognizer-readme?view=azure-dotnet&viewFallbackFrom=doc-intel-4.0.0#examples&preserve-view=true)
- Azure Document Intelligence client library for .NET [GitHub](https://github.com/Azure/azure-sdk-for-net/tree/Azure.AI.DocumentIntelligence_1.0.0-beta.2/sdk/documentintelligence/Azure.AI.DocumentIntelligence)


## Setup

### Prerequisites

- .NET 6.0 or higher
- An Azure subscription
- An Azure AI Document Intelligence resource (formerly known as Form Recognizer)
- Visual Studio or any C# IDE

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/AzureAiDocumentIntelligenceMay2024.git
   cd AzureAiDocumentIntelligenceMay2024

2. Add required packages, remember to use the "prerelease" option as per below or the "Include prerelease" checkbox in Visual Studio Manage NuGet Packages gui.
   Most recent version of Azure.AI.DocumentIntelligence is 1.0.0-beta.2 March 5, 2024.
   ```c#
   dotnet add package Microsoft.Extensions.Configuration
   dotnet add package Microsoft.Extensions.Configuration.Json
   dotnet add package Microsoft.Extensions.DependencyInjection
   dotnet add package Azure.AI.DocumentIntelligence --prerelease
   dotnet add package Azure.Identity
   ```
3. Create a Document Intelligence/Form Recognizer resource in the Portal or with AZ CLI:
   You may wish to use the F0 SKU for limted free use while experimenting.
   ```
   az cognitiveservices account create \
    --name <resource-name> \
    --resource-group <resource-group-name> \
    --kind FormRecognizer \
    --sku <sku> \
    --location <location> \
    --yes
    ```
4. Get the endpoint for this resource in the portal by opening the resource and looking in "Resource Management > Keys and Endpoint". Or with by using the Az CLI:
   ```
   az cognitiveservices account show --name "<resource-name>" --resource-group "<resource-group-name>" --query "properties.endpoint"
   ```
5. Use the API key to connect or managed identity. For managed identity connections add "Cognitive Services User" 
   role to your tenant user for this resource. This project uses managed identity to connect.
   ```
   az cognitiveservices account keys list --name "<resource-name>" --resource-group "<resource-group-name>"
   ```

### Remember
The message the client gives when it cannot find a model, the responding error message "resource not found" is easy to misinterpret as the local client not being able to communicate with the Azure AI.
