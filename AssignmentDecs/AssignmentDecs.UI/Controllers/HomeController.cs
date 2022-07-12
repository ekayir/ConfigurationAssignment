using AssignmentDecs.Data;
using AssignmentDecs.Data.Enums;
using AssignmentDecs.Service;
using AssignmentDecs.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentDecs.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IConfigurationService _configurationService;
        private ConfigurationReader _configurationReader;
        public HomeController(ILogger<HomeController> logger, IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _configurationReader = new ConfigurationReader("App1", "", 2000);
        }

        public IActionResult Index(string applicationName, bool isSuccess = true, string actionMessage = null)
        {
            if (applicationName == null) return View(new ConfigurationViewModel { ConfigurationTypeIdentities = _configurationService.GetConfigurationTypeIdentities() });

            return View(new ConfigurationViewModel
            {
                ApplicationName = applicationName,
                ConfigurationDTOs = _configurationService.GetConfigurations(applicationName, true),
                ActionResult = new Result { IsSuccess = isSuccess, Message = actionMessage },
                ConfigurationTypeIdentities = _configurationService.GetConfigurationTypeIdentities()
            });

        }
        public IActionResult AddEditConfiguration(string id_applicationName, string id_name, string id_value, ConfigurationTypeIdentity id_type, bool id_isActive, int id_id)
        {
            var result = new Result<ConfigurationDTO>();

            if (id_id == 0) result = _configurationService.AddConfiguration(new ConfigurationDTO
            {
                ApplicationName = id_applicationName,
                IsActive = id_isActive,
                Name = id_name,
                Type = id_type,
                Value = id_value
            });
            else result = _configurationService.EditConfiguration(new ConfigurationDTO
            {
                ApplicationName = id_applicationName,
                IsActive = id_isActive,
                Name = id_name,
                Type = id_type,
                Value = id_value,
                Id = id_id
            });

            return RedirectToAction("Index", "Home", new { applicationName = id_applicationName, isSuccess = result.IsSuccess, actionMessage = result.Message });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
