using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Core.Modules;
using MORR.Shared.Modules;
using MORRTest.TestHelper.Modules;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;

namespace MORRTest.Modules
{
    [TestClass]
    public class ModuleManagerTest
    {
        public class TestModuleOne : TestModule { }
        public class TestModuleTwo : TestModule { }

        private TestModuleOne module1;
        private TestModuleTwo module2;
        private ModuleManager manager;
        private GlobalModuleConfiguration config;
        private CompositionContainer container;

        [TestInitialize]
        public void BeforeTest()
        {
            manager = new ModuleManager();
            module1 = new TestModuleOne();
            module2 = new TestModuleTwo();
        }

        [TestCleanup]
        public void AfterTest()
        {
            container.Dispose();
            manager = null;
            module1 = null;
            module2 = null;
            container = null;
        }

        /// <summary>
        /// Tests whether the ModuleManager works as expected if no module is enabled.
        /// </summary>
        [TestMethod]
        public void TestModuleManager_NoEnabledModule()
        {
            /* GIVEN */
            config = new GlobalModuleConfiguration
            {
                EnabledModules = new Type[] { }
            };

            Compose();

            /* PRECONDITION */
            Debug.Assert(module1?.Mock != null);
            Debug.Assert(module2?.Mock != null);
            Debug.Assert(manager != null);

            /* WHEN */
            manager.InitializeModules();

            /* THEN */
            module1.Mock.Verify(mock => mock.Initialize(false));
            module2.Mock.Verify(mock => mock.Initialize(false));
        }

        /// <summary>
        /// Tests if the ModuleManager correctly initializes modules if not all loaded are active.
        /// </summary>
        [TestMethod]
        public void TestModuleManager_SingleModuleEnabled()
        {
            /* GIVEN */
            config = new GlobalModuleConfiguration
            {
                EnabledModules = new[] { typeof(TestModuleOne) }
            };

            Compose();

            /* PRECONDITION */
            Debug.Assert(module1?.Mock != null);
            Debug.Assert(module2?.Mock != null);
            Debug.Assert(manager != null);

            /* WHEN */
            manager.InitializeModules();

            /* THEN */
            module1.Mock.Verify(mock => mock.Initialize(true));
            module2.Mock.Verify(mock => mock.Initialize(false));
        }

        /// <summary>
        /// Tests whether the ModuleManager can enable multiple modules.
        /// </summary>
        [TestMethod]
        public void TestModuleManager_MultipleModulesEnabled()
        {
            /* GIVEN */
            config = new GlobalModuleConfiguration
            {
                EnabledModules = new[] { typeof(TestModuleOne), typeof(TestModuleTwo) }
            };

            Compose();

            /* PRECONDITION */
            Debug.Assert(module1?.Mock != null);
            Debug.Assert(module2?.Mock != null);
            Debug.Assert(manager != null);

            /* WHEN */
            manager.InitializeModules();

            /* THEN */
            module1.Mock.Verify(mock => mock.Initialize(true));
            module2.Mock.Verify(mock => mock.Initialize(true));
        }

        /// <summary>
        /// Tests if the NotifyModulesOnSessionStart correctly notifies all enabled modules about
        /// the start of an session.
        /// </summary>
        [TestMethod]
        public void TestModuleManager_NotifyModulesOnSessionStart()
        {
            /* GIVEN */
            config = new GlobalModuleConfiguration
            {
                EnabledModules = new[] { typeof(TestModuleTwo) }
            };

            Compose();

            /* PRECONDITION */
            Debug.Assert(module1?.Mock != null);
            Debug.Assert(module2?.Mock != null);
            Debug.Assert(manager != null);

            /* WHEN */
            manager.InitializeModules();
            manager.NotifyModulesOnSessionStart();

            /* THEN */
            module1.Mock.VerifySet(mock => mock.IsActive = true, Times.Never);
            module2.Mock.VerifySet(mock => mock.IsActive = true, Times.Once);
        }

        /// <summary>
        /// Tests if the NotifyModulesOnSessionStop correctly notifies all enabled modules about
        /// the stop of the currently running session.
        /// </summary>
        [TestMethod]
        public void TestModuleManager_NotifyModulesOnSessionStop()
        {
            /* GIVEN */
            config = new GlobalModuleConfiguration
            {
                EnabledModules = new[] { typeof(TestModuleTwo) }
            };

            Compose();

            /* PRECONDITION */
            Debug.Assert(module1?.Mock != null);
            Debug.Assert(module2?.Mock != null);
            Debug.Assert(manager != null);

            /* WHEN */
            manager.InitializeModules();
            manager.NotifyModulesOnSessionStop();

            /* THEN */
            module1.Mock.VerifySet(mock => mock.IsActive = false, Times.Never);
            module2.Mock.VerifySet(mock => mock.IsActive = false, Times.Once);
        }

        /// <summary>
        /// Composes the ModuleManager using MEF and loads two instances
        /// of the test modules.
        /// </summary>
        private void Compose()
        {
            container = new CompositionContainer();
            container.ComposeExportedValue<IModule>(module1);
            container.ComposeExportedValue<IModule>(module2);
            container.ComposeExportedValue(config);
            container.ComposeParts(manager);
        }
    }
}
