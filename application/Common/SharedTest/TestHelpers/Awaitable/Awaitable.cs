using System.Threading;

namespace SharedTest.TestHelpers.AsyncEnumerable
{
    public static class Awaitable
    {
        public static T Await<T>(T awaitedObject, ManualResetEvent expectation)
        {
            expectation.Set();
            return awaitedObject;
        }
    }
}
