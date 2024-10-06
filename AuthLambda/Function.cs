using Amazon.CognitoIdentityProvider;
using Amazon.Lambda.Core;
using AuthLambda.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.APIGatewayEvents;
using AuthLambda.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AuthLambda;

public class Function
{
    private readonly ServiceProvider _serviceProvider;

    public Function()
    {
        // Configurar a leitura do appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Pega o diret�rio atual
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Configura��o do cont�iner de depend�ncias
        var serviceCollection = new ServiceCollection();

        // Registrar o IConfiguration
        serviceCollection.AddSingleton<IConfiguration>(configuration);

        // Registrar o AmazonCognitoIdentityProviderClient
        serviceCollection.AddAWSService<IAmazonCognitoIdentityProvider>();

        // Registrar o AuthService
        serviceCollection.AddTransient<AuthService>();

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public async Task<APIGatewayProxyResponse> FunctionHandler(AuthRequest request, ILambdaContext context)
    {
        // Resolver o AuthService a partir do cont�iner de depend�ncias
        var authService = _serviceProvider.GetService<AuthService>();

        // Chamar o m�todo de autentica��o
        return await authService.AuthenticateUserAsync(request, context);
    }
}
