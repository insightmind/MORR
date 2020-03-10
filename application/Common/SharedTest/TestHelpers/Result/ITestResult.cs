using System;
using System.Threading;

namespace SharedTest.TestHelpers.Result
{
    /// <summary>
    /// This marks an accessible validation type for validation of
    /// an asynchronous task result.
    /// </summary>
    public interface ITestResult
    {
        /// <summary>
        /// Returns whether the Test did finish successfully
        /// </summary>
        /// <returns>True if the result marks successfully execution.</returns>
        public bool IsSuccess();

        /// <summary>
        /// Asserts whether the result marks a successfully completed execution.
        /// </summary>
        public void AssertSuccess();

        /// <summary>
        /// Asserts whether the result marks an execution which resulted in the given
        /// type of exception to be thrown.
        /// </summary>
        /// <typeparam name="T">The type of exception expected to be thrown by the execution</typeparam>
        public void AssertThrows<T>() where T : Exception;

        /// <summary>
        /// Sets or Resets the given ResetEvent depending whether the execution did complete successfully or not.
        /// If it failed or did not complete the Event will be reset.
        /// </summary>
        /// <param name="resetEvent">The ResetEvent to set or reset given the condition.</param>
        public void EventSuccess(ManualResetEvent resetEvent);

        /// <summary>
        /// Sets or Resets the given ResetEvent depending whether the execution completed with an exception or not.
        /// If it failed with the given type of exception the Event will be set to true.
        /// </summary>
        /// <typeparam name="T">The type of exception expected to be thrown.</typeparam>
        /// <param name="resetEvent">The ResetEvent to set or reset given the condition.</param>
        public void EventThrows<T>(ManualResetEvent resetEvent) where T : Exception;
    }
}
