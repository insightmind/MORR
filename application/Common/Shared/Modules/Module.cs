using System;

namespace MORR.Shared.Modules
{
    public abstract class Module: IModule
    {
        public bool IsActive { get; set; }
        public Guid Identifier { get; }
        public void Initialize() { /* The default implementation is empty */ }
    }
}
