using Microsoft.Extensions.Configuration;

namespace AuthLambda.Configuration
{
    public class AwsSettings
    {
        public string UserPoolId { get; set; }
        public string Region { get; set; }

        public AwsSettings(IConfiguration configuration)
        {
            UserPoolId = configuration["AWS:UserPoolId"];
            Region = configuration["AWS:Region"];
        }
    }
}
