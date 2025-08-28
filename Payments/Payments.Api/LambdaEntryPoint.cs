using Amazon.Lambda.AspNetCoreServer;

namespace Payments.Api;

/// <summary>
/// Lambda entry point for the Payments API
/// This is used only when deployed as a Lambda function
/// </summary>
public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory())
               .UseStartup<Startup>();
    }
}
