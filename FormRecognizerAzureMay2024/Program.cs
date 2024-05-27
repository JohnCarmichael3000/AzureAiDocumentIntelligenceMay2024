using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormRecognizerAzureMay2024
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Program...");

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Set up the dependency injection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            Console.WriteLine($"Calling {nameof(MsExampleFormRecognizer)}.{nameof(MsExampleFormRecognizer.analyzeDocument)}() method.");
            var formRecognizer = serviceProvider.GetService<MsExampleFormRecognizer>() ?? throw new Exception($"Unable to find service {nameof(MsExampleFormRecognizer)}.");
            //await formRecognizer.analyzeDocument();

            Console.WriteLine($"Calling {nameof(MsExampleDocIntelligence)}.{nameof(MsExampleDocIntelligence.AnalyzeDocumentContent)}() method.");
            var docIntelligence = serviceProvider.GetService<MsExampleDocIntelligence>() ?? throw new Exception($"Unable to find service {nameof(MsExampleDocIntelligence)}.");
            await docIntelligence.AnalyzeDocumentContent();

            Console.WriteLine($"{Environment.NewLine}*******************************************{Environment.NewLine}[Program Completed - Press any key to exit]{Environment.NewLine}");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddTransient<MsExampleFormRecognizer>();
            services.AddTransient<MsExampleDocIntelligence>();
        }
    }

}
