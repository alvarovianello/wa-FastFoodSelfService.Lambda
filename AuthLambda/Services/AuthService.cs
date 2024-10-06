using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using AuthLambda.Models;
using Amazon;

namespace AuthLambda.Services
{
    public class AuthService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly string _userPoolId;

        public AuthService(IAmazonCognitoIdentityProvider cognitoProvider, IConfiguration configuration)
        {
            _cognitoClient = new AmazonCognitoIdentityProviderClient(RegionEndpoint.GetBySystemName(configuration["AWS:Region"]));

            _userPoolId = configuration["AWS:UserPoolId"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<APIGatewayProxyResponse> AuthenticateUserAsync(AuthRequest request, ILambdaContext context)
        {
                var cpf = request.Cpf;
                var defaultUser = new { CPF = "12345678901", Name = "Usuário Padrão" };

                if (string.IsNullOrEmpty(cpf))
                {
                    cpf = defaultUser.CPF;
                }

                try
                {
                    var user = await GetUserByCpfAsync(cpf);
                    if (user == null)
                    {
                        return GenerateResponse(404, "Usuário não encontrado.");
                    }

                    // Retorna os detalhes do usuário
                    return GenerateResponse(200, JsonSerializer.Serialize(user));
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Erro ao buscar o usuário no Cognito: {ex.Message}");
                    return GenerateResponse(500, "Erro interno ao validar o usuário.");
                }

        }
        private async Task<AdminGetUserResponse> GetUserByCpfAsync(string cpf)
        {
            try
            {
                var request = new ListUsersRequest
                {
                    UserPoolId = _userPoolId,
                    Filter = $"username = \"{cpf}\""
                };

                var response = await _cognitoClient.ListUsersAsync(request);

                if (response.Users.Count > 0)
                {
                    var user = response.Users[0];
                    return new AdminGetUserResponse
                    {
                        Username = user.Username,
                        UserAttributes = user.Attributes
                    };
                }
                return null;
            }
            catch (UserNotFoundException)
            {
                return null;
            }
        }
        private APIGatewayProxyResponse GenerateResponse(int statusCode, string body)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
