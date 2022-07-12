using AssignmentDecs.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AssignmentDecs.Service
{
    public class ConfigurationReader 
    {
        private IConfigurationService _configurationService;
        private static List<ConfigurationDTO> _configurations { get; set; }
        private string _applicationName { get; set; }
        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            Init(applicationName, connectionString);

            CancellationTokenSource tokenSource = new CancellationTokenSource();

            var checkNewMembersTask = RunPeriodically(RefreshDataFromStorage, TimeSpan.FromMilliseconds(refreshTimerIntervalInMs), tokenSource.Token);
        }
        private void Init(string applicationName, string connectionString)
        {
            _applicationName = applicationName;

            if (_configurationService == null)
            {
                _configurationService = new ConfigurationService(new AssignmentDecsDBEntities());
            }
        }

        async Task RunPeriodically(Action action, TimeSpan interval, CancellationToken token)
        {
            while (true)
            {
                action();
                await Task.Delay(interval, token);
            }
        }
        private void RefreshDataFromStorage()
        {
            var newConfigurations = GetDataFromStorage();

            if (newConfigurations.IsSuccess == true)
            {
                _configurations = newConfigurations.Data;
            }
        }

        private Result<List<ConfigurationDTO>> GetDataFromStorage()
        {
            try
            {
                return new Result<List<ConfigurationDTO>> { IsSuccess = true, Data = _configurationService.GetConfigurations(_applicationName, false) };
            }
            catch (Exception ex)
            {
                return Result<List<ConfigurationDTO>>.FromException(ex, null);
            }
        }

        public T GetValue<T>(string key)
        {
            var configuration = _configurations.FirstOrDefault(x => x.Name == key);

            if (configuration == null) throw new Exception("No configuration KeyValuePair related to " + key);

            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(configuration?.Value);
        }
    }
}
