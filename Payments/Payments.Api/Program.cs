using Amazon.Lambda.AspNetCoreServer.Hosting;

namespace Payments.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Check if running in Lambda environment
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME")))
        {
            // Running locally - use Kestrel
            await CreateHostBuilder(args).Build().RunAsync();
        }
        else
        {
            // Running in Lambda
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
            
            var app = builder.Build();
            startup.Configure(app, app.Environment);
            
            await app.RunAsync();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

