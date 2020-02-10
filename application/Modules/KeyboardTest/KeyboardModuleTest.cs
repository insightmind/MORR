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
        private KeyboardModule keyboardModule;

        [TestInitialize]
        public void BeforeTest()
        {
            keyboardModule = new KeyboardModule();
        }

        [TestMethod]
        public void TestKeyboardModule_OnActivate()
        {
            // Preconditions
            Debug.Assert(keyboardModule != null);

            /* GIVEN */
            var keyboardInteractEventProducerMock = new Mock<KeyboardInteractEventProducer>();
            var container = new CompositionContainer();
            container.ComposeExportedValue(keyboardInteractEventProducerMock.Object);
            container.ComposeParts(keyboardModule);

            keyboardInteractEventProducerMock.Setup(mock => mock.StartCapture());

            /* WHEN */
            keyboardModule.IsActive = true;

            /* THEN */
            Assert.IsTrue(keyboardModule.IsActive);
            keyboardInteractEventProducerMock.Verify(mock => mock.StartCapture(), Times.Once());
        }
    }
}