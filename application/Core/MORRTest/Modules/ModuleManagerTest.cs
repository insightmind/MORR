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


        [TestInitialize]
        public void BeforeTest()
        {
            manager = new ModuleManager();
            module1 = new TestModuleOne();
            module2 = new TestModuleTwo();
        }

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

        private void Compose()
        {
            var container = new CompositionContainer();
            container.ComposeExportedValue<IModule>(module1);
            container.ComposeExportedValue<IModule>(module2);
            container.ComposeExportedValue(config);
            container.ComposeParts(manager);
        }
    }
}
