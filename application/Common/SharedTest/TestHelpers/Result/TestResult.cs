using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharedTest.TestHelpers.Result
{
    /// <summary>
    /// TestResult is a simple class which allows simple validation of an asynchronous task.
    /// It also can capture exceptions which may occurred till the completion call.
    ///
    /// Please use the provided assertion methods or event validation methods
    /// to resolve the state of the result.
    ///
    /// This class implements the ITestResult interface so the caller is only able to
    /// validate the action without actually set a validation afterwards.
    /// </summary>
    public class TestResult: ITestResult
    {
        private bool didComplete;
        private Exception exception;

        /// <summary>
        /// Sets the result as successfully completed without an exception.
        /// </summary>
        public void Complete()
        {
            didComplete = true;
        }

        /// <summary>
        /// Sets the result as completed. However the given exception was thrown.
        /// </summary>
        /// <param name="thrownException"></param>
        public void Fail(Exception thrownException)
        {
            didComplete = true;
            exception = thrownException;
        }

        public bool IsSuccess()
        {
            return didComplete && exception == null;
        }

        /// <summary>
        /// Asserts whether the result marks a successfully completed execution.
        /// </summary>
        public void AssertSuccess()
        {
            Assert.IsTrue(didComplete);
            Assert.IsNull(exception);
        }

        /// <summary>
        /// Asserts whether the result marks an execution which resulted in the given
        /// type of exception to be thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception expected to be thrown by the execution</typeparam>
        public void AssertThrows<T>() where T : Exception
        {
            Assert.IsTrue(didComplete);
            Assert.IsInstanceOfType(exception, typeof(T));
        }

        /// <summary>
        /// Sets or Resets the given ResetEvent depending whether the execution did complete successfully or not.
        /// If it failed or did not complete the Event will be reset.
        /// </summary>
        /// <param name="resetEvent">The ResetEvent to set or reset given the condition.</param>
        public void EventSuccess(ManualResetEvent resetEvent)
        {
            Debug.Assert(resetEvent != null);

            if (IsSuccess())
            {
                resetEvent.Set();
            }
            else
            {
                resetEvent.Reset();
            }
        }

        /// <summary>
        /// Sets or Resets the given ResetEvent depending whether the execution completed with an exception or not.
        /// If it failed with the given type of exception the Event will be set to true.
        /// </summary>
        /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
        /// <param name="resetEvent">The ResetEvent to set or reset given the condition.</param>
        public void EventThrows<T>(ManualResetEvent resetEvent) where T : Exception
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
