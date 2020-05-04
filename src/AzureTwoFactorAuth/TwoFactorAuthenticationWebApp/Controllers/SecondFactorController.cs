namespace TwoFactorAuthenticationWebApp.Controllers
{
    using Framework.Services.Storage;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Devices;
    using Microsoft.Azure.Devices.Common.Exceptions;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using TwoFactorAuthentication.Services.Contracts.Entities;
    using TwoFactorAuthenticationWebApp.Models;

    public class SecondFactorController : Controller
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

        public SecondFactorController(IConfiguration configuration)
        {
            this.configuration = configuration;
            ServiceClientConnectionString = this.configuration[ServiceClientConnectionStringName];
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            // Remove previous settings to ensure new settings are applied.
            if(settings[FrameworkServicesStorageStorageConnectionStringName] != null)
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

        public IActionResult Index()
        {            // Create a ServiceClient to communicate with service-facing endpoint on your hub.
            serviceClient = ServiceClient.CreateFromConnectionString(ServiceClientConnectionString);
            var uniqueId = Guid.NewGuid().ToString();
            var correlationId = Guid.NewGuid().ToString("N");
            InvokeMethod("admin", uniqueId, correlationId).GetAwaiter().GetResult();
            CreateTableAndAddRequest("admin", uniqueId, correlationId).GetAwaiter().GetResult();

            var secondFactorModel = new SecondFactorModel()
            {
                PartitionKey = "admin",
                RowKey = uniqueId,
                CorrelationId = correlationId,

            };

            return View(secondFactorModel);
        }

        #endregion Actions

        #region Private methods

        // Invoke the direct method on the device, passing the payload
        private async Task InvokeMethod(string userName, string uniqueId, string correlationId)
        {
            var methodInvocation = new CloudToDeviceMethod("LedColorControlMethod") { ResponseTimeout = TimeSpan.FromSeconds(60) };
            var dataString = $"{userName}|{uniqueId}|{correlationId}";
            var payLoadData = "{\"color\": \"red\", \"data\": \"" + dataString + "\"}";
            methodInvocation.SetPayloadJson(payLoadData);
            var deviceId = ConfigurationManager.AppSettings[DeviceIdConfigurationName];
            // Invoke the direct method asynchronously and get the response from the simulated device.
            try
            {
                var response = await serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation);
                responseBody = response.GetPayloadAsJson();
            }
            catch (DeviceNotFoundException dnfe)
            {
                Debug.WriteLine(dnfe);
            }
        }

        private async Task CreateTableAndAddRequest(string partitionKey, string rowKey, string correlationId)
        {
            var createTableTask = tableService.CreateTableAsync(RequestTable);
            TwoFactorRequestEntity tableEntity = new TwoFactorRequestEntity()
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                CorrelationId = correlationId,
                RequestCreationTimestamp = DateTime.Now.ToString()
            };
            var insertResult = await tableService.InsertEntityAsync(RequestTable, tableEntity);
        }

        #endregion Private methods
    }
}