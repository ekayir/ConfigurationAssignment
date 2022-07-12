using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentDecs.Data
{
    public partial class Configuration
    {
        public ConfigurationDTO ConvertToDTO()
        {
            return new ConfigurationDTO
            {
                ApplicationName = this.ApplicationName,
                Id = this.Id,
                IsActive = this.IsActive,
                Name = this.Name,
                Type = this.Type,
                Value = this.Value
            };
        }
    }
}
