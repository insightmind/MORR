using System;
using System.Collections.Generic;
using System.Text;

namespace MORR.Shared.Utility.Exceptions
{
    public class HookLibraryException : Exception {

        public HookLibraryException() { }
        public HookLibraryException(string message) : base(message) { }
    }
}
