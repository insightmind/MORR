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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Threading;

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

        private void ConfigureDefaultConfig(bool usingDecoder = true)
        {
            Debug.Assert(config != null);

            if (usingDecoder)
            {
                config.Decoders = new[] { typeof(TestDecoder) };
            }
            
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

        /// <summary>
        /// Tests whether the session manager starts a recording session correctly.
        /// </summary>
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

        /// <summary>
        /// Tests whether starting an already running recording session throws an error.
        /// In this case the session manager should throw an AlreadyRecordingException.
        /// </summary>
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

        /// <summary>
        /// Tests whether stopping a running recording session works as expected.
        /// </summary>
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

        /// <summary>
        /// Tests whether the session manager throws if a not running recording session is stopped.
        /// The session manager should throw in this case a NotRecordingException.
        /// </summary>
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

        /// <summary>
        /// Tests whether processing with multiple directories works as expected!
        /// </summary>
        [TestMethod]
        public void TestSessionManager_ProcessCorrectlyMultipleDirectories()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed prematuraly!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");
            Debug.Assert(decoder?.Mock != null, "TestEncoder should not be dismissed!");

            encoder.Mock.SetupGet(mock => mock.EncodeFinished)?
                .Returns(new ManualResetEvent(true));

            decoder.Mock.SetupGet(mock => mock.DecodeFinished)?
                .Returns(new ManualResetEvent(true));

            const string directoryPath = "someDirectory";
            var someDirectory = new DirectoryPath(directoryPath, true);
            var someOtherDirectory = new DirectoryPath(directoryPath, true);
            var directories = new[] { someDirectory, someOtherDirectory };
            var count = directories.Length;

            /* WHEN */
            Assert.IsFalse(sessionManager.isRecording);
            sessionManager.Process(directories);

            /* THEN */
            Debug.Assert(moduleManagerMock != null, "ModuleManager should not be dismissed!");

            decoder.Mock.Verify(mock => mock.Decode(It.IsAny<DirectoryPath>()), Times.Exactly(count));
            decoder.Mock.VerifyGet(mock => mock.DecodeFinished, Times.Exactly(count));

            encoder.Mock.Verify(mock => mock.Encode(It.IsAny<DirectoryPath>()), Times.Exactly(count));
            encoder.Mock.VerifyGet(mock => mock.EncodeFinished, Times.Exactly(count));

            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStart(), Times.Once);
            moduleManagerMock.Verify(mock => mock.NotifyModulesOnSessionStop(), Times.Once);
        }

        /// <summary>
        /// Tests whether processing fails if not decoder has been specified!
        /// </summary>
        [TestMethod]
        public void TestSessionManager_NoDecoder()
        {
            /* GIVEN */
            ConfigureDefaultConfig(usingDecoder: false);
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed prematuraly!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");

            const string directoryPath = "someDirectory";
            var someDirectory = new DirectoryPath(directoryPath, true);
            var directories = new[] { someDirectory };

            /* WHEN */
            Assert.IsFalse(sessionManager.isRecording);
            Assert.ThrowsException<InvalidConfigurationException>(() => sessionManager.Process(directories));
        }

        /// <summary>
        /// Tests whether processing fails if the session manager is already running a recording session.
        /// </summary>
        [TestMethod]
        public void TestSessionManager_AlreadyRecording()
        {
            /* GIVEN */
            ConfigureDefaultConfig();
            InitializeSessionManager();

            Debug.Assert(sessionManager != null, "SessionManager should not be dismissed prematuraly!");
            Debug.Assert(encoder?.Mock != null, "TestEncoder should not be dismissed!");

            const string directoryPath = "someDirectory";
            var someDirectory = new DirectoryPath(directoryPath, true);
            var directories = new[] { someDirectory };


            Assert.IsFalse(sessionManager.isRecording);
            sessionManager.StartRecording();
            Assert.IsTrue(sessionManager.isRecording);

            /* WHEN */
            Assert.ThrowsException<AlreadyRecordingException>(() => sessionManager.Process(directories));
        }
    }
}
