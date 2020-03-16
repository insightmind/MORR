using Microsoft.VisualStudio.TestTools.UnitTesting;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Configuration;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace MORRTest.Configuration
{
    [TestClass]
    public class ConfigurationManagerTest
    {
        private IConfigurationManager configManager;
        private MockFileSystem fileSystem;
        private CompositionContainer container;

        private const string defaultPath = "C:\\temp\\config.morr";

        [TestInitialize]
        public void BeforeTest()
        {
            fileSystem = new MockFileSystem();
            configManager = new ConfigurationManager(fileSystem);
            container = new CompositionContainer();
        }

        [TestCleanup]
        public void AfterTest()
        {
            container.Dispose();
            configManager = null;
            fileSystem = null;
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
            var mockData = new MockFileData("{}");
            fileSystem.AddFile(defaultPath, mockData);

            /* WHEN */
            try
            {
                configManager.LoadConfiguration(new FilePath(defaultPath, true));
            }
            catch (Exception exception)
            {
                Assert.Fail("ConfigManager did throw an error (" + exception.Message + ") however an empty config should still be sufficient");
            }
        }

        [TestMethod]
        public void TestConfigurationManager_AssignConfig()
        {
            /* PRECONDITION */
            Debug.Assert(configManager != null);
            Debug.Assert(fileSystem != null);
            Debug.Assert(container != null);

            /* GIVEN */
            var configType = typeof(TestConfiguration).FullName;
            const string testConfig = "{\n\"isEnabled\": true\n}";
            var fullConfig = "{\n\"" + configType + "\":" + testConfig + "\n}";

            var configurationMock = new TestConfiguration(new RawConfiguration(testConfig));
            container.ComposeExportedValue<IConfiguration>(configurationMock);
            container.ComposeParts(configManager);

            var mockData = new MockFileData(fullConfig);
            fileSystem.AddFile(defaultPath, mockData);

            /* WHEN */
            try
            {
                configManager.LoadConfiguration(new FilePath(defaultPath, true));
            }
            catch (Exception exception)
            {
                Assert.Fail("ConfigManager did throw an error (" + exception.Message + ") however was supplied with valid config.");
            }

            /* THEN */
            Debug.Assert(configurationMock.TestResult != null);
            configurationMock.TestResult.AssertSuccess();
        }

        [TestMethod]
        public void TestConfigurationManager_AssignUnknownConfig()
        {
            /* PRECONDITION */
            Debug.Assert(configManager != null);
            Debug.Assert(fileSystem != null);
            Debug.Assert(container != null);

            /* GIVEN */
            // We use the normal name here as this does not uniquely identify the type
            var configType = typeof(TestConfiguration).Name;
            const string testConfig = "{\n\"isEnabled\": true\n}";
            var fullConfig = "{\n\"" + configType + "\":" + testConfig + "\n}";

            var configurationMock = new TestConfiguration(new RawConfiguration(testConfig));
            
            container.ComposeExportedValue<IConfiguration>(configurationMock);
            container.ComposeParts(configManager);

            var mockData = new MockFileData(fullConfig);
            fileSystem.AddFile(defaultPath, mockData);

            /* WHEN */
            Assert.ThrowsException<InvalidConfigurationException>(() => configManager.LoadConfiguration(new FilePath(defaultPath, true)));
        }
    }
}
