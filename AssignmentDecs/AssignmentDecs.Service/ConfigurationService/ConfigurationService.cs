using AssignmentDecs.Data;
using AssignmentDecs.Data.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssignmentDecs.Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly AssignmentDecsDBEntities _context;
        public ConfigurationService(AssignmentDecsDBEntities context)
        {
            _context = context;
        }

        public Result<ConfigurationDTO> AddConfiguration(ConfigurationDTO configurationDTO)
        {
            try
            {
                if (configurationDTO == null) return new Result<ConfigurationDTO> { IsSuccess = false, Message = ActionMessage.NotNullMessage };

                var isValid = IsValidAddEdit(configurationDTO.ApplicationName, configurationDTO.Name, null, configurationDTO.Value, configurationDTO.Type, configurationDTO.IsActive);

                if (isValid.IsSuccess == false) return new Result<ConfigurationDTO> { IsSuccess = false, Data = configurationDTO, Message = isValid.Message };


                var configuration = configurationDTO.ConvertToDAO();

                using (var transaction = _context.Database.BeginTransaction())
                {
                    configuration.Id = ((_context.Configurations.Max(x => (int?)x.Id) ?? 0) + 1);
                    _context.Configurations.Add(configuration);
                    _context.Database.ExecuteSqlRaw(@"SET IDENTITY_INSERT [dbo].[Configurations] ON");
                    _context.SaveChanges();
                    _context.Database.ExecuteSqlRaw(@"SET IDENTITY_INSERT [dbo].[Configurations] OFF");
                    transaction.Commit();
                }

                return new Result<ConfigurationDTO> { IsSuccess = true, Data = configuration.ConvertToDTO(), Message = ActionMessage.AddingSuccessfullMessage };
            }
            catch (Exception ex)
            {
                return Result<ConfigurationDTO>.FromException(ex, configurationDTO);
            }
        }

        public Result<ConfigurationDTO> EditConfiguration(ConfigurationDTO configurationDTO)
        {
            try
            {
                if (configurationDTO == null) return new Result<ConfigurationDTO> { IsSuccess = false, Message = ActionMessage.NotNullMessage };

                var configuration = GetConfiguration(configurationDTO.Id);

                if (configuration == null) return new Result<ConfigurationDTO> { IsSuccess = false, Message = string.Format(ActionMessage.NoConfiguratonRelatedNameMessage, configurationDTO.Name) };

                var isValid = IsValidAddEdit(configurationDTO.ApplicationName, configurationDTO.Name, configurationDTO.Id, configurationDTO.Value, configurationDTO.Type, configurationDTO.IsActive);

                if (isValid.IsSuccess == false) return new Result<ConfigurationDTO> { IsSuccess = false, Data = configurationDTO, Message = isValid.Message };

                configuration.Name = configurationDTO.Name;
                configuration.Value = configurationDTO.Value;
                configuration.IsActive = configurationDTO.IsActive;
                configuration.Type = configurationDTO.Type;

                _context.SaveChanges();

                return new Result<ConfigurationDTO> { IsSuccess = true, Data = configurationDTO, Message = ActionMessage.UpdatingSuccessfullMessage };
            }
            catch (Exception ex)
            {
                return Result<ConfigurationDTO>.FromException(ex, configurationDTO);
            }
        }

        public List<ConfigurationDTO> GetConfigurations(string applicationName, bool includeNotActive)
        {
            using (var db = new AssignmentDecsDBEntities())
            {
                return db.Configurations
                           .Where(x => x.ApplicationName == applicationName &&
                                       (includeNotActive == true || x.IsActive == true))
                           .Select(x => x.ConvertToDTO())
                           .ToList();
            }
        }
        public Result DeleteAllConfigurations()
        {
            try
            {
                var configurations = _context.Configurations.ToList();
                _context.Configurations.RemoveRange(configurations);
                _context.SaveChanges();

                return new Result { IsSuccess = true };
            }
            catch (Exception)
            {
                return new Result { IsSuccess = false };
            }
        }
        public List<ConfigurationTypeIdentity> GetConfigurationTypeIdentities()
        {
            return Enum.GetValues(typeof(ConfigurationTypeIdentity))
                       .Cast<ConfigurationTypeIdentity>()
                       .ToList();
        }

        #region Helper Functions
        private Result<bool> IsValidAddEdit(string applicationName, string name, int? id, string value, ConfigurationTypeIdentity type, bool isActive)
        {
            if (isActive == true)
            {
                var active = _context.Configurations.FirstOrDefault(x => x.ApplicationName == applicationName && x.Name == name && x.IsActive == true);

                if (active != null && ((id.HasValue == false) || (id.HasValue == true && active.Id != id))) return new Result<bool> { IsSuccess = false, Message = ActionMessage.ExistAnotherActiveRecordMessage };
            }

            if (IsValidTypeAndValue(value, type) == false) return new Result<bool> { IsSuccess = false, Message = ActionMessage.IncompatibleTypeAndValueMessage };

            return new Result<bool> { IsSuccess = true };
        }
        private bool IsValidTypeAndValue(string value, ConfigurationTypeIdentity type)
        {
            switch (type)
            {
                case ConfigurationTypeIdentity.String:
                    return true;
                case ConfigurationTypeIdentity.Boolean:
                    return bool.TryParse(value, out bool boo);
                case ConfigurationTypeIdentity.Int:
                    return int.TryParse(value, out int n);
                case ConfigurationTypeIdentity.Decimal:
                    return decimal.TryParse(value, out decimal dec);
                case ConfigurationTypeIdentity.Double:
                    return double.TryParse(value, out double dou);
                case ConfigurationTypeIdentity.DateTime:
                    return DateTime.TryParse(value, out DateTime dat);
                default:
                    return false;
            }
        }
        private Configuration GetConfiguration(int id)
        {
            return _context.Configurations.FirstOrDefault(x => x.Id == id);
        }
        #endregion
    }
}
