using System;

namespace MORR.Shared.Utility.Exceptions
{
    public class HookLibraryException : Exception {

        public HookLibraryException() { }
        public HookLibraryException(string message) : base(message) { }
    }
}
