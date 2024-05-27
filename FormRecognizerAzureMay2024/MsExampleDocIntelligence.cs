using Azure;
using Azure.Identity;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration;

namespace FormRecognizerAzureMay2024
{
    internal class MsExampleDocIntelligence
    {
        private readonly IConfiguration _configuration;
        private static string _endpoint = string.Empty;
        private static string _modelid = string.Empty;
        private static string _uripath = string.Empty;

        public MsExampleDocIntelligence(IConfiguration configuration)
        {
            _configuration = configuration;
            _endpoint = _configuration["endpoint"] ?? throw new Exception("endpoint key and value in form https://[YourDocIntelligenceResourceName].cognitiveservices.azure.com/ is required.");
            _modelid = _configuration["modelidInvoice"] ?? throw new Exception("modelid is required. Your custom ModelIds can be found within in your projects at https://documentintelligence.ai.azure.com/studio/custommodel/projects");

            //"https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-invoice.pdf"
            _uripath = configuration["UriInvoicePdfPath"] ?? throw new Exception("URI Path to invoice pdf is required.");
        }

        public async Task AnalyzeDocumentContent()
        {
            Uri fileUri = new Uri(_uripath);

            DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(_endpoint), new DefaultAzureCredential());

            Console.WriteLine($"AnalyzeDocumentFromUriAsync starting with:\r\nendpoint {_endpoint}\r\nmodelId {_modelid}\r\nfileUri {fileUri}");

            AnalyzeDocumentContent content = new AnalyzeDocumentContent()
            {
                UrlSource = fileUri
            };

            Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-invoice", content);

            AnalyzeResult result = operation.Value;

            Console.WriteLine($"KeyValuePairs found: {result.KeyValuePairs.Count.ToString()}");
            foreach (var oneKey in result.KeyValuePairs)
            {
                Console.WriteLine(oneKey.Key + "=" + oneKey.Value);
            }


            for (int i = 0; i < result.Documents.Count; i++)
            {
                AnalyzedDocument document = result.Documents[i];
                Console.WriteLine($"Document {i} containing {document.Fields.Count} document.Fields.");

                if (document.Fields.TryGetValue("VendorName", out DocumentField? vendorNameField)
                    && vendorNameField.Type == DocumentFieldType.String)
                {
                    string vendorName = vendorNameField.ValueString;
                    Console.WriteLine($"Vendor Name: '{vendorName}', with confidence {vendorNameField.Confidence}");
                }

                if (document.Fields.TryGetValue("CustomerName", out DocumentField? customerNameField)
                    && customerNameField.Type == DocumentFieldType.String)
                {
                    string customerName = customerNameField.ValueString;
                    Console.WriteLine($"Customer Name: '{customerName}', with confidence {customerNameField.Confidence}");
                }

                //if (document.Fields.TryGetValue("Items", out DocumentField itemsField)
                //    && itemsField.Type == DocumentFieldType.Array)
                //{
                //    foreach (DocumentField itemField in itemsField.ValueArray)
                //    {
                //        Console.WriteLine("Item:");

                //        if (itemField.Type == DocumentFieldType.Object)
                //        {
                //            IReadOnlyDictionary<string, DocumentField> itemFields = itemField.ValueObject;

                //            if (itemFields.TryGetValue("Description", out DocumentField itemDescriptionField)
                //                && itemDescriptionField.Type == DocumentFieldType.String)
                //            {
                //                string itemDescription = itemDescriptionField.ValueString;
                //                Console.WriteLine($"  Description: '{itemDescription}', with confidence {itemDescriptionField.Confidence}");
                //            }

                //            if (itemFields.TryGetValue("Amount", out DocumentField itemAmountField)
                //                && itemAmountField.Type == DocumentFieldType.Currency)
                //            {
                //                CurrencyValue itemAmount = itemAmountField.ValueCurrency;
                //                Console.WriteLine($"  Amount: '{itemAmount.CurrencySymbol}{itemAmount.Amount}', with confidence {itemAmountField.Confidence}");
                //            }
                //        }
                //    }
                //}

                if (document.Fields.TryGetValue("SubTotal", out DocumentField? subTotalField)
                    && subTotalField.Type == DocumentFieldType.Currency)
                {
                    CurrencyValue subTotal = subTotalField.ValueCurrency;
                    Console.WriteLine($"Sub Total: '{subTotal.CurrencySymbol}{subTotal.Amount}', with confidence {subTotalField.Confidence}");
                }

                if (document.Fields.TryGetValue("TotalTax", out DocumentField? totalTaxField)
                    && totalTaxField.Type == DocumentFieldType.Currency)
                {
                    CurrencyValue totalTax = totalTaxField.ValueCurrency;
                    Console.WriteLine($"Total Tax: '{totalTax.CurrencySymbol}{totalTax.Amount}', with confidence {totalTaxField.Confidence}");
                }

                if (document.Fields.TryGetValue("InvoiceTotal", out DocumentField? invoiceTotalField)
                    && invoiceTotalField.Type == DocumentFieldType.Currency)
                {
                    CurrencyValue invoiceTotal = invoiceTotalField.ValueCurrency;
                    Console.WriteLine($"Invoice Total: '{invoiceTotal.CurrencySymbol}{invoiceTotal.Amount}', with confidence {invoiceTotalField.Confidence}");
                }
            }
        }

    }
}
