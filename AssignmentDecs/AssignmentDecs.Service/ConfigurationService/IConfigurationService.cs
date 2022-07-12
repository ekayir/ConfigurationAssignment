using AssignmentDecs.Data;
using AssignmentDecs.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentDecs.Service
{
    public interface IConfigurationService
    {
        List<ConfigurationDTO> GetConfigurations(string applicationName, bool includeNotActive);
        Result<ConfigurationDTO> AddConfiguration(ConfigurationDTO configurationDTO);
        Result<ConfigurationDTO> EditConfiguration(ConfigurationDTO configurationDTO);
        List<ConfigurationTypeIdentity> GetConfigurationTypeIdentities();
        Result DeleteAllConfigurations();
    }
}
