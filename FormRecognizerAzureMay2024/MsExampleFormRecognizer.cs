using Azure;
using Azure.Identity;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;

namespace FormRecognizerAzureMay2024
{
    internal class MsExampleFormRecognizer
    {
        private readonly IConfiguration _configuration;
        private static string _endpoint = string.Empty;
        private static string _modelid = string.Empty;
        private static string _uripath = string.Empty;
        private readonly List<string> _fieldOrder;

        public MsExampleFormRecognizer(IConfiguration configuration)
        {
            _configuration = configuration;
            _endpoint = _configuration["endpoint"] ?? throw new Exception("endpoint key and value in form https://[YourDocIntelligenceResourceName].cognitiveservices.azure.com/ is required.");
            _modelid = _configuration["modelid"] ?? throw new Exception("modelid is required. Your custom ModelIds can be found within in your projects at https://documentintelligence.ai.azure.com/studio/custommodel/projects");
            _uripath = configuration["uripath"] ?? throw new Exception("URI Path to image is required.");
            _fieldOrder = _configuration.GetSection("fieldOrder").Get<List<string>>() ?? throw new Exception("fieldOrder is required in appsettings.json");
        }

        public async Task analyzeDocument()
        {
            Uri fileUri = new Uri(_uripath);

            DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(_endpoint), new DefaultAzureCredential());

            Console.WriteLine($"AnalyzeDocumentFromUriAsync starting with:\r\nendpoint {_endpoint}\r\nmodelId {_modelid}\r\nfileUri {fileUri}");
            AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, _modelid, fileUri);
            AnalyzeResult result = operation.Value;

            Console.WriteLine($"Document was analyzed with model with ID: {result.ModelId}");

            foreach (AnalyzedDocument document in result.Documents)
            {
                Console.WriteLine($"Document of type: {document.DocumentType}");

                // fieldKvp.Key, fieldKvp.Value;
                foreach (var fieldName in _fieldOrder)
                {
                    if (document.Fields.TryGetValue(fieldName, out DocumentField? field))
                    {
                        Console.WriteLine($"Key: {fieldName} = {field?.Content}");
                    }
                }

                string? keyGroupName = _configuration["KeyGroupName1"];
                Console.WriteLine($"Key: {keyGroupName} = {GetSelectedStatus(document.Fields, keyGroupName)}");

                keyGroupName = _configuration["KeyGroupName2"];
                Console.WriteLine($"Key: {keyGroupName} = {GetSelectedStatus(document.Fields, keyGroupName)}");
            }
        }

        public static string? GetSelectedStatus(IReadOnlyDictionary<string, DocumentField> fields, string? keyGroupName)
        {
            if (keyGroupName == null)
            {
                return null;
            }
            foreach (var fieldKvp in fields)
            {
                if (fieldKvp.Key.StartsWith(keyGroupName) && fieldKvp.Value.Content == ":selected:")
                {
                    string status = fieldKvp.Key.Replace(keyGroupName, "");
                    return status;
                }
            }
            return $"{keyGroupName} = Not Found"; // Return a default message if no selected status is found
        }

    }
}
