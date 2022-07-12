using AssignmentDecs.Data;
using AssignmentDecs.Data.Enums;
using AssignmentDecs.Service;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AssignmentDecs.Test
{
    public class MainTest
    {
        IConfigurationService _configurationService;
        private static string conn1 = "Server=.\\SQLEXPRESS;Database=AssignmentDecsDB;Trusted_Connection=True;";
        public MainTest()
        {
            var services = new ServiceCollection();
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<AssignmentDecsDBEntities, AssignmentDecsDBEntities>();

            var serviceProvider = services.BuildServiceProvider();

            _configurationService = serviceProvider.GetService<IConfigurationService>();

            _configurationService.DeleteAllConfigurations();//just before running test, old records should be deleted.
        }
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void AA_NoRecordRelatedApplicationNameTest()
        {
            var actualResult = _configurationService.GetConfigurations("ABCDEF123456", false);
            var expectedResult = new List<ConfigurationDTO>();

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void AB_SuccessfullAddingTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "SiteName",
                Value = "google.com",
                Type = ConfigurationTypeIdentity.String
            };

            var expectedResult = true;
            var actualResult = _configurationService.AddConfiguration(newRecord);

            Assert.AreEqual(expectedResult, actualResult.IsSuccess);
        }

        [Test]
        public void AC_ExistAnotherActiveRecordTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "SiteName",
                Value = "google.com",
                Type = ConfigurationTypeIdentity.String
            };

            var expectedResult = new Result { IsSuccess = false, Message = ActionMessage.ExistAnotherActiveRecordMessage };
            var processResult = _configurationService.AddConfiguration(newRecord);

            Assert.IsTrue((expectedResult.IsSuccess == processResult.IsSuccess) && (expectedResult.Message == processResult.Message));
        }

        [Test]
        public void AD_SuccessfullUpdateTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "SiteName",
                Value = "google.co.uk",
                Type = ConfigurationTypeIdentity.String,
                Id = 1
            };

            var expectedResult = new Result<ConfigurationDTO> { IsSuccess = true, Data = newRecord, Message = ActionMessage.UpdatingSuccessfullMessage };
            var processResult = _configurationService.EditConfiguration(newRecord);

            Assert.IsTrue((expectedResult.IsSuccess == processResult.IsSuccess) && (expectedResult.Message == processResult.Message));
        }

        [Test]
        public void AE_NullValueTestForAdding()
        {
            var expectedResult = new Result<ConfigurationDTO> { IsSuccess = false, Message = ActionMessage.NotNullMessage };
            var processResult = _configurationService.AddConfiguration(null);

            Assert.IsTrue((expectedResult.IsSuccess == processResult.IsSuccess) && (expectedResult.Message == processResult.Message));
        }

        [Test]
        public void AF_NullValueTestForEditing()
        {
            var expectedResult = new Result<ConfigurationDTO> { IsSuccess = false, Message = ActionMessage.NotNullMessage };
            var processResult = _configurationService.EditConfiguration(null);

            Assert.IsTrue((expectedResult.IsSuccess == processResult.IsSuccess) && (expectedResult.Message == processResult.Message));
        }

        [Test]
        public void AG_NoConfigurationTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "SiteName",
                Value = "google.co.uk",
                Type = ConfigurationTypeIdentity.String,
                Id = 10000
            };

            var expectedResult = new Result<ConfigurationDTO> { IsSuccess = false, Message = string.Format(ActionMessage.NoConfiguratonRelatedNameMessage, newRecord.Name) };
            var processResult = _configurationService.EditConfiguration(newRecord);

            Assert.IsTrue((expectedResult.IsSuccess == processResult.IsSuccess) && (expectedResult.Message == processResult.Message));
        }


        [Test]
        public void AR_ConfigurationReaderTest()
        {
            //this is a periodic controlling test. in 200ms , data refreshed. 
            
            _configurationService.DeleteAllConfigurations();

            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "SiteName",
                Value = "google.com",
                Type = ConfigurationTypeIdentity.String
            };

            var configuration = _configurationService.AddConfiguration(newRecord);
            newRecord.Id = configuration.Data.Id;

            var configurationReader = new ConfigurationReader("App1", conn1, 200);

            var oldSiteName = configurationReader.GetValue<string>("SiteName");

            newRecord.Value = "google.co.uk";

            _configurationService.EditConfiguration(newRecord);

            Thread.Sleep(2000);

            var newSiteName = configurationReader.GetValue<string>("SiteName");

            Assert.IsTrue((oldSiteName == "google.com") && (newSiteName == "google.co.uk"));
        }

        [Test]
        public void AX0_BooleanTypeTrueTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "IsCurrent",
                Value = "true",
                Type = ConfigurationTypeIdentity.Boolean
            };

            var configuration = _configurationService.AddConfiguration(newRecord);

            var configurationReader = new ConfigurationReader("App1", conn1, 2000);

            var actualResult = configurationReader.GetValue<bool>("IsCurrent");

            Assert.AreEqual(true, actualResult);
        }

        [Test]
        public void AX1_BooleanTypeFalseTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = false,
                Name = "IsCurrent",
                Value = "false",
                Type = ConfigurationTypeIdentity.Boolean
            };

            var configuration = _configurationService.AddConfiguration(newRecord);

            var configurationReader = new ConfigurationReader("App1", conn1, 2000);

            var actualResult = configurationReader.GetValue<bool>("IsCurrent");

            Assert.AreEqual(true, actualResult);
        }


        [Test]
        public void AY_DateTimeTypeTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "LastUpdateDate",
                Value = "2022-07-12",
                Type = ConfigurationTypeIdentity.DateTime
            };

            var configuration = _configurationService.AddConfiguration(newRecord);

            var configurationReader = new ConfigurationReader("App1", conn1, 2000);

            var actualResult = configurationReader.GetValue<DateTime>("LastUpdateDate");

            Assert.AreEqual(new DateTime(2022, 7, 12), actualResult);
        }

        [Test]
        public void AZ_DecimalTypeTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "DiscountRate",
                Value = "0.5",
                Type = ConfigurationTypeIdentity.Decimal
            };

            var configuration = _configurationService.AddConfiguration(newRecord);

            var configurationReader = new ConfigurationReader("App1", conn1, 2000);

            var actualResult = configurationReader.GetValue<decimal>("DiscountRate");
            var expectedResult = 0.5m;

            Assert.AreEqual(expectedResult, actualResult);
        }


        [Test]
        public void AZ_DoubleTypeTest()
        {
            var newRecord = new ConfigurationDTO
            {
                ApplicationName = "App1",
                IsActive = true,
                Name = "DiscountRate",
                Value = "0.5",
                Type = ConfigurationTypeIdentity.Double
            };

            var configuration = _configurationService.AddConfiguration(newRecord);

            var configurationReader = new ConfigurationReader("App1", conn1, 2000);

            var actualResult = configurationReader.GetValue<Double>("DiscountRate");
            double expectedResult = 0.5;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void BigDataTest()
        {
            _configurationService.DeleteAllConfigurations();

            for (int i = 0; i < 100; i++)
            {
                var newRecord = new ConfigurationDTO
                {
                    ApplicationName = (i % 2 == 0) ? "App1" : "App2",
                    IsActive = (i % 2 == 0) ? true : false,
                    Name = "SiteName" + i,
                    Value = "google.com",
                    Type = ConfigurationTypeIdentity.String
                };

                _configurationService.AddConfiguration(newRecord);
            }

            var app1All = _configurationService.GetConfigurations("App1", true);
            var app1Actives = _configurationService.GetConfigurations("App1", false);

            var app2All = _configurationService.GetConfigurations("App2", true);
            var app2Actives = _configurationService.GetConfigurations("App2", false);

            Assert.IsTrue(app1Actives.Count == 50 && app1All.Count == 50 && app2Actives.Count == 0 && app2All.Count == 50 );
        }
    }
}