namespace RecorderAzureFunction
{
    using Framework.Services.Storage;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Configuration;
    using TwoFactorAuthentication.Services.Contracts.Entities;
    using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

    /// <summary>
    /// Captures messages coming into Azure IoT hub from Azure Sphere device.
    /// </summary>
    public static class DeviceMessageRecorderFunction
    {
        private static string key = TelemetryConfiguration.Active.InstrumentationKey = System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY", EnvironmentVariableTarget.Process);
        private static TelemetryClient telemetry = new TelemetryClient()
        {
            InstrumentationKey = key
        };

        [FunctionName("EventHubTrigger")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "EventHubConnectionAppSetting")] string message, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"IoT Hub trigger function processed a message: {message}");
            // telemetry.TrackTrace($"IoT Hub trigger function processed a message: {message}");
            //SetEnvironment(context, log);
            
            #region Storage connection

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var storageConnectionString = config["Framework.Services.Storage.StorageConnectionString"];

            #endregion Storage connection

            log.LogInformation($"IoT Hub trigger function --- completed setting of environment");
            TableService tableService = new TableService(storageConnectionString);
            log.LogInformation($"IoT Hub trigger function --- Created new instance of tableService");
            if (message.Contains("|"))
            {
                var messageArray = message.Split("|");
                // Check if the message is for retrieved security code or created security code. 5 means it is retrieved, 4 means it is created.
                log.LogInformation($"IoT Hub trigger function --- splitted into message array");
                // Update data store for the created security code
                if (messageArray.Length == 4)
                {
                    log.LogInformation($"IoT Hub trigger function --- splitted into message array -- In the four");
                    var singleEntityTask = tableService.GetSingleEntityAsync<TwoFactorRequestEntity>("TwoFactorRequest", messageArray[0], messageArray[1]);
                    var retrievedEntity = singleEntityTask.Result;
                    retrievedEntity.CreatedSecurityCode = messageArray[3];
                    retrievedEntity.RequestCreationTimestamp = System.DateTime.Now.ToString();
                    log.LogInformation($"IoT Hub trigger function --- About toupdate tableService sync");
                    var updateEntityTask = tableService.UpdateEntityAsync("TwoFactorRequest", retrievedEntity);
                }

                // Update data store for the retrieved (validated) security code
                if (messageArray.Length == 5)
                {
                    log.LogInformation($"IoT Hub trigger function --- splitted into message array -- In the five");
                    var singleEntityTask = tableService.GetSingleEntityAsync<TwoFactorRequestEntity>("TwoFactorRequest", messageArray[0], messageArray[1]);
                    var retrievedEntity = singleEntityTask.Result;
                    retrievedEntity.RetrievedSecurityCode = messageArray[3];
                    retrievedEntity.RequestValidatedTimestamp = System.DateTime.Now.ToString();
                    log.LogInformation($"IoT Hub trigger function --- About toupdate tableService sync");
                    var updateEntityTask = tableService.UpdateEntityAsync("TwoFactorRequest", retrievedEntity);
                }
            }
        }

        #region Private methods

        private static void SetEnvironment(ExecutionContext context, ILogger log)
        {
            log.LogInformation($"In SetEnvironment: Invocation Id -> {context.InvocationId} --> Function directory {context.FunctionDirectory} --> Function Name {context.FunctionName} --> Function AppDirectory {context.FunctionAppDirectory}");
            log.LogInformation($"In SetEnvironment: --- About to build configuration");
            // Getting configuration settings.
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                //.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            log.LogInformation($"In SetEnvironment: --- Created configuration");
            var serviceClientConnectionString = config["ServiceClientConnectionString"];
            log.LogInformation($"In SetEnvironment: --- Got serviceClientConnectionString");
            var storageConnectionString = config["Framework.Services.Storage.StorageConnectionString"];
            log.LogInformation($"In SetEnvironment: --- Got storageConnectionString");
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            log.LogInformation($"In SetEnvironment: --- Opened configuration file");
            var settings = configFile.AppSettings.Settings;
            log.LogInformation($"In SetEnvironment: --- About to save config");
            configFile.Save(ConfigurationSaveMode.Modified);
            log.LogInformation($"In SetEnvironment: serviceClientConnectionString -> {serviceClientConnectionString} --> storageConnectionString {storageConnectionString} --> configFile {configFile.FilePath} --> Function AppDirectory {context.FunctionAppDirectory}");

            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            log.LogInformation($"In SetEnvironment: --- Completed save 1 About to save refresh");
            if (settings["ServiceClientConnectionString"] != null)
            {
                settings.Remove("ServiceClientConnectionString");
            }
            if (settings["Framework.Services.Storage.StorageConnectionString"] != null)
            {
                settings.Remove("Framework.Services.Storage.StorageConnectionString");
            }
            settings.Add("ServiceClientConnectionString", serviceClientConnectionString);
            settings.Add("Framework.Services.Storage.StorageConnectionString", storageConnectionString);
            log.LogInformation($"In SetEnvironment: --- About to save config2");
            configFile.Save(ConfigurationSaveMode.Modified);
            log.LogInformation($"In SetEnvironment: --- Completed save 2 About to save refresh");
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            log.LogInformation($"In SetEnvironment: refreshed Section -> {configFile.AppSettings.SectionInformation.Name}");
        }

        #endregion Private methods
    }
}