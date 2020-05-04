namespace TwoFactorAuthenticationWebApp.Controllers
{
    using System;
    using Framework.Services.Storage;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Devices;
    using Microsoft.Extensions.Configuration;
    using System.Configuration;
    using TwoFactorAuthentication.Services.Contracts.Entities;

    public class ValidatorController : Controller
    {
        #region Private members

        private IConfiguration configuration;
        private static ServiceClient serviceClient;
        private string responseBody = string.Empty;
        static readonly TableService tableService = new TableService();
        private const string DeviceIdConfigurationName = "DeviceId";
        private const string ServiceClientConnectionStringName = "ServiceClientConnectionString";
        private const string FrameworkServicesStorageStorageConnectionStringName = "Framework.Services.Storage.StorageConnectionString";
        private const string RequestTable = "TwoFactorRequest";
        // Connection string for the IoT Hub
        // az iot hub show-connection-string --hub-name {your iot hub name}
        private readonly string ServiceClientConnectionString = "HostName=NabeelAzureSphereHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=YJo5RVykLb/YdFpZwvR31wnl+Of/rU9WGKfMLWH/8aY=";

        #endregion Private members

        #region Constructor(s)

        public ValidatorController(IConfiguration configuration)
        {
            this.configuration = configuration;
            ServiceClientConnectionString = this.configuration[ServiceClientConnectionStringName];
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            // Remove previous settings to ensure new settings are applied.
            if (settings[FrameworkServicesStorageStorageConnectionStringName] != null)
            {
                settings.Remove(FrameworkServicesStorageStorageConnectionStringName);
            }
            if (settings[DeviceIdConfigurationName] != null)
            {
                settings.Remove(DeviceIdConfigurationName);
            }
            if (settings[ServiceClientConnectionStringName] != null)
            {
                settings.Remove(ServiceClientConnectionStringName);
            }

            settings.Add(FrameworkServicesStorageStorageConnectionStringName, this.configuration[FrameworkServicesStorageStorageConnectionStringName]);
            settings.Add(DeviceIdConfigurationName, this.configuration[DeviceIdConfigurationName]);
            settings.Add(ServiceClientConnectionStringName, this.configuration[ServiceClientConnectionStringName]);
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        #endregion Constructor(s)

        #region Actions

        /// <summary>
        /// Action to validate request
        /// </summary>
        /// <param name="partionKey"></param>
        /// <param name="rowKey"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public IActionResult Index(string partionKey, string rowKey, string correlationId)
        {
            return Ok(ValidateSecondFactorAuthentication(partionKey, rowKey, correlationId));
        }

        #endregion Actions

        #region Private methods

        private bool ValidateSecondFactorAuthentication(string partionKey, string rowKey, string correlationId)
        {
            var singleEntityTask = tableService.GetSingleEntityAsync<TwoFactorRequestEntity>("TwoFactorRequest", partionKey, rowKey);
            var requestedEntity = singleEntityTask.Result;
            if (requestedEntity != null && requestedEntity.CorrelationId == correlationId)
            {
                // Validate
                if(!string.IsNullOrEmpty(requestedEntity.CreatedSecurityCode) && requestedEntity.CreatedSecurityCode.Equals(requestedEntity.RetrievedSecurityCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion Private methods
    }
}