using System;

namespace MORR.Shared.Hook.Exceptions
{
    public class HookLibraryException : Exception {
        public HookLibraryException(string message) : base(message) { }
    }
}
