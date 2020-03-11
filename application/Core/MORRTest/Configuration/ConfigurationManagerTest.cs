using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using MORR.Shared.Utility;

namespace MORRTest.Configuration
{
    [TestClass]
    public class ConfigurationManagerTest
    {
        public IConfigurationManager configManager;
        private readonly MockFileSystem fileSystem = new MockFileSystem();

        [TestInitialize]
        public void BeforeTest()
        {
            configManager = new ConfigurationManager(fileSystem);
        }

        [TestMethod]
        public void TestConfigurationManager_NullPath()
        {
            /* PRECONDITION */
            Debug.Assert(configManager != null);
            Assert.ThrowsException<ArgumentNullException>(() => configManager.LoadConfiguration(null));
        }

        [TestMethod]
        public void TestConfigurationManager_InvalidPath()
        {
            /* PRECONDITION */
            Debug.Assert(configManager != null);

            /* GIVEN */
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var invalidPath = Path.GetDirectoryName(assemblyPath) + "\\config.morr";
            var filePath = new FilePath(invalidPath, true);

            /* THEN */
            Assert.ThrowsException<FileNotFoundException>(() => configManager.LoadConfiguration(filePath));
        }

        [TestMethod]
        public void TestConfigurationManager_EmptyConfig()
        {
            /* PRECONDITION */
            Debug.Assert(configManager != null);
            Debug.Assert(fileSystem != null);

            /* GIVEN */
            const string path = "C:\\temp\\config.morr";
            var mockData = new MockFileData("{}");
            fileSystem.AddFile(path, mockData);

            var filePath = new FilePath(path, true);
            
            /* WHEN */
            try
            {
                configManager.LoadConfiguration(filePath);
            }
            catch (Exception exception)
            {
                Assert.Fail("ConfigManager did throw an error (" + exception.Message + ") however an empty config should still be sufficient");
            }
        }
    }
}
