using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.TestHelpers
{
    public class TestResult
    {
        private bool didComplete;
        private Exception? exception;

        public void Complete()
        {
            didComplete = true;
        }

        public void Fail(Exception exception)
        {
            didComplete = true;
            this.exception = exception;
        }

        public void AssertDidCompleteSuccessfully()
        {
            Assert.IsTrue(didComplete);
            Assert.IsNull(exception);
        }

        public void AssertDidCompleteThrowing<T>() where T : Exception
        {
            Assert.IsTrue(didComplete);
            Assert.IsInstanceOfType(exception, typeof(T));
        }

        public void WasSuccess(ManualResetEvent resetEvent)
        {
            Debug.Assert(resetEvent != null);

            if (didComplete && exception == null)
            {
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();
            }
        }

        public void DidFailThrowing<T>(ManualResetEvent resetEvent) where T : Exception
        {
            Debug.Assert(resetEvent != null);

            if (exception?.GetType() == typeof(T))
            {
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();
            }
        }
    }
}
