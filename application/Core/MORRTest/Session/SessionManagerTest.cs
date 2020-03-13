using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core;
using MORR.Core.Configuration;
using MORR.Core.Data.Transcoding;
using MORR.Core.Modules;
using MORR.Core.Session;
using MORR.Core.Session.Exceptions;
using MORR.Shared.Utility;
using MORRTest.TestHelper.Decoder;
using MORRTest.TestHelper.Encoder;
using SharedTest.TestHelpers.INativeHook;

namespace MORRTest.Session
{
    [TestClass]
    public class SessionManagerTest
    {
        private SessionManager sessionManager;

        /* MOCKS */
        private CompositionContainer container;
        private FilePath testFilePath;
        private SessionConfiguration config;
        private TestEncoder encoder;
        private TestDecoder decoder;
        private Mock<IModuleManager> moduleManagerMock;
        private Mock<IConfigurationManager> configurationManagerMock;
        private Mock<IBootstrapper> bootstrapperMock;
        private MockFileSystem fileSystemMock;

        [TestInitialize]
        public void BeforeTest()
        {
            container = new CompositionContainer();
            testFilePath = new FilePath(@"C:\test", true);
            config = new SessionConfiguration();
            encoder = new TestEncoder();
            decoder = new TestDecoder();

            moduleManagerMock = new Mock<IModuleManager>();
            configurationManagerMock = new Mock<IConfigurationManager>();
            fileSystemMock = new MockFileSystem();
            bootstrapperMock = new Mock<IBootstrapper>();

            var hookMock = new HookNativeMethodsMock();
            hookMock.AllowLibraryLoad();
        }

        private void PrepareBootstrapper()
        {
            Debug.Assert(container != null);
            Debug.Assert(bootstrapperMock != null);

            bootstrapperMock.Setup(mock => mock.ComposeImports(It.IsAny<object>()))?
                .Callback<object>((composeObject) => container.ComposeParts(composeObject));
        }

        private void ConfigureDefaultConfig()
        {
            Debug.Assert(config != null);
            config.Decoders = new[] { typeof(TestDecoder) };
            config.Encoders = new[] { typeof(TestEncoder) };

            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            var validPath = Path.GetDirectoryName(assemblyPath) + "\\";

            config.RecordingDirectory = new DirectoryPath(validPath);
        }

        private void InitializeSessionManager()
        {
            Debug.Assert(encoder != null);
            Debug.Assert(decoder != null);
            Debug.Assert(config != null);
            Debug.Assert(container != null);

            container.ComposeExportedValue<IEncoder>(encoder);
            container.ComposeExportedValue<IDecoder>(decoder);
            container.ComposeExportedValue(config);
            PrepareBootstrapper();

            Debug.Assert(bootstrapperMock?.Object != null);
            Debug.Assert(moduleManagerMock?.Object != null);
            Debug.Assert(configurationManagerMock?.Object != null);
            Debug.Assert(fileSystemMock != null);

            sessionManager = new SessionManager(testFilePath, bootstrapperMock.Object, configurationManagerMock.Object, moduleManagerMock.Object, fileSystemMock);
        }

        /// <summary>
        /// Tests if the initializer correctly executes on correct parameters.
        /// </summary>
        [TestMethod]
        public void TestSessionManager_CorrectInitialization()
        {
            /* GIVEN */
            ConfigureDefaultConfig();

            /* WHEN */
            InitializeSessionManager();

            /* THEN */
            Debug.Assert(configurationManagerMock != null, "ConfigurationManagerMock should not be dismissed!");

            Assert.IsNotNull(sessionManager, "SessionManager unexpectedly null!");
            configurationManagerMock.Verify(mock => mock.LoadConfiguration(testFilePath), Times.Once);
        }

        [TestMethod]
        public void TestSessionManager_StartRecordingCorrectly()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            /* WHEN */
            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed!");
            sessionManager.StartRecording();

            /* THEN */
            Debug.Assert(moduleManagerMock != null, "ModuleManagerMock should not be dismissed!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");

            Assert.IsTrue(sessionManager.isRecording);
            Assert.IsNotNull(sessionManager.RecordingsFolder);

            moduleManagerMock.Verify(mock => mock.InitializeModules(), Times.Once);
            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStart(), Times.Once);
            encoder.Mock.Verify(mock => mock.Encode(It.IsAny<DirectoryPath>()), Times.Once);
        }

        [TestMethod]
        public void TestSessionManager_ThrowsAlreadyRunning()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed!");
            sessionManager.StartRecording();

            /* WHEN */
            Assert.IsTrue(sessionManager.isRecording);
            Assert.ThrowsException<AlreadyRecordingException>(() => sessionManager.StartRecording());

            /* THEN */
            Debug.Assert(moduleManagerMock != null, "ModuleManagerMock should not be dismissed!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");

            Assert.IsTrue(sessionManager.isRecording);

            moduleManagerMock.Verify(mock => mock.InitializeModules(), Times.Once);
            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStart(), Times.Once);
            encoder.Mock.Verify(mock => mock.Encode(It.IsAny<DirectoryPath>()), Times.Once);
        }

        [TestMethod]
        public void TestSessionManager_StopRecordingCorrectly()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");
            sessionManager.StartRecording();

            encoder.Mock.SetupGet(mock => mock.EncodeFinished)?
                .Returns(new ManualResetEvent(true));

            /* WHEN */
            sessionManager.StopRecording();

            /* THEN */
            Debug.Assert(moduleManagerMock != null, "ModuleManagerMock should not be dismissed!");

            Assert.IsFalse(sessionManager.isRecording);

            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStop(), Times.Once);
            encoder.Mock.VerifyGet(mock => mock.EncodeFinished, Times.Once);
        }

        [TestMethod]
        public void TestSessionManager_ThrowsAlreadyStopped()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed!");

            /* WHEN */
            Assert.IsFalse(sessionManager.isRecording);
            Assert.ThrowsException<NotRecordingException>(() => sessionManager.StopRecording());

            /* THEN */
            Debug.Assert(moduleManagerMock != null, "ModuleManagerMock should not be dismissed!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");

            Assert.IsFalse(sessionManager.isRecording);

            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStop(), Times.Never);
            encoder.Mock.VerifyGet(mock => mock.EncodeFinished, Times.Never);
        }
    }
}
