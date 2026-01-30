using Microsoft.Extensions.Configuration;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ProviderPayments.Republish.Function.Services
{
    public class AzureFunctionConfigurationHelper : IConfigurationHelper
    {
        private readonly IConfiguration _configuration;

        public AzureFunctionConfigurationHelper(IConfiguration configuration)
        { 
            _configuration = configuration;
        }

        public bool HasSetting(string sectionName, string settingName)
        {
            return true;
        }

        public string GetSetting(string sectionName, string settingName)
        {
            return _configuration[settingName];
        }
    }
}
