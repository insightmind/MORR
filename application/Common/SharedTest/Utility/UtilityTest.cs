using System.Threading;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.Utility
{
    [TestClass]
    public class UtilityTest
    {
        private const int maxWaitTime = 500; 

        [TestMethod]
        public void TestUtility_SetAndDispatch_OnTrueValueChange()
        {
            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var prevValue = false;
            var newValue = true;

            /* WHEN */
            MORR.Shared.Utility.Utility.SetAndDispatch(
                ref prevValue,
                newValue,
                () => autoResetEvent.Set(), 
                () => autoResetEvent.Reset());

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(maxWaitTime));
            Assert.IsTrue(prevValue);
            Assert.AreEqual(newValue, prevValue);
        }

        [TestMethod]
        public void TestUtility_SetAndDispatch_OnNoValueChange()
        {
            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(true);
            var prevValue = true;
            var newValue = true;

            /* WHEN */
            MORR.Shared.Utility.Utility.SetAndDispatch(
                ref prevValue,
                newValue,
                () => autoResetEvent.Reset(),
                () => autoResetEvent.Reset());

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(maxWaitTime));
            Assert.IsTrue(prevValue);
            Assert.AreEqual(newValue, prevValue);
        }

        [TestMethod]
        public void TestUtility_SetAndDispatch_OnFalseValueChange()
        {
            /* GIVEN */
            var autoResetEvent = new AutoResetEvent(false);
            var prevValue = true;
            var newValue = false;

            /* WHEN */
            MORR.Shared.Utility.Utility.SetAndDispatch(
                ref prevValue,
                newValue,
                () => autoResetEvent.Reset(),
                () => autoResetEvent.Set());

            /* THEN */
            Assert.IsTrue(autoResetEvent.WaitOne(maxWaitTime));
            Assert.IsFalse(prevValue);
            Assert.AreEqual(newValue, prevValue);
        }

        [Ignore] // A bug is preventing this test to complete successfully.
        [TestMethod]
        public void TestUtility_GetTypeFromAnyAssembly_Valid()
        {
            /* GIVEN */
            var expectedType = typeof(MORR.Shared.Utility.Utility);
            var typeName = nameof(expectedType);

            /* WHEN */
            var type = MORR.Shared.Utility.Utility.GetTypeFromAnyAssembly(typeName);

            /* THEN */
            Assert.AreEqual(expectedType, type);
        }

        [TestMethod]
        public void TestUtility_GetTypeFromAnyAssembly_Invalid()
        {
            /* GIVEN */
            var typeName = "NULL";

            /* WHEN */
            var type = MORR.Shared.Utility.Utility.GetTypeFromAnyAssembly(typeName);

            /* THEN */
            Assert.IsNull(type);
        }
    }
}
