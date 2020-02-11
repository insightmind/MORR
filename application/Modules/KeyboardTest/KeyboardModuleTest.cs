using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MORR.Modules.Keyboard;
using MORR.Modules.Keyboard.Producers;

namespace KeyboardTest
{
    [TestClass]
    public class KeyboardModuleTest
    {
        private CompositionContainer container;
        private Mock<KeyboardInteractEventProducer> keyboardInteractEventProducer;
        private KeyboardModule keyboardModule;

        [TestInitialize]
        public void BeforeTest()
        {
            keyboardModule = new KeyboardModule();
            keyboardInteractEventProducer = new Mock<KeyboardInteractEventProducer>();
            container = new CompositionContainer();
            container.ComposeExportedValue(keyboardInteractEventProducer.Object);
            container.ComposeParts(keyboardModule);
        }

        [TestMethod]
        public void TestKeyboardModule_Activate()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(keyboardModule.IsActive);
        }

        [TestMethod]
        public void TestKeyboardModule_Deactivate()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */

            /* WHEN */
            keyboardModule.IsActive = false;

            /* THEN */
            Assert.IsFalse(keyboardModule.IsActive);
        }
    }
}