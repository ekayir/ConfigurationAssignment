using AssignmentDecs.Data;
using AssignmentDecs.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentDecs.UI.Models
{
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel()
        {
            ConfigurationDTOs = new List<ConfigurationDTO>();
            ActionResult = new Result { IsSuccess = true };
        }
        public List<ConfigurationDTO> ConfigurationDTOs { get; set; }
        public string ApplicationName { get; set; }
        public Result ActionResult { get; set; }
        public List<ConfigurationTypeIdentity> ConfigurationTypeIdentities { get; set; }
    }
}
