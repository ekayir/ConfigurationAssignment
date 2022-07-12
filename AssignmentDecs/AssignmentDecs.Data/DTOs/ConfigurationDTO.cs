using AssignmentDecs.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AssignmentDecs.Data
{
    public class ConfigurationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ConfigurationTypeIdentity Type { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public string ApplicationName { get; set; }

        public Configuration ConvertToDAO()
        {
            return new Configuration
            {
                ApplicationName = this.ApplicationName,
                IsActive = this.IsActive,
                Name = this.Name,
                Type = this.Type,
                Value = this.Value
            };
        }
    }
}
