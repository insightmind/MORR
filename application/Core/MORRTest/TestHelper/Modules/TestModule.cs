using System;
using System.Diagnostics;
using Moq;
using MORR.Shared.Modules;

namespace MORRTest.TestHelper.Modules
{
    public class TestModule : IModule
    {
        public readonly Mock<IModule> Mock = new Mock<IModule>();

        public bool IsActive
        {
            get
            {
                Debug.Assert(Mock?.Object != null);
                return Mock.Object.IsActive;
            }
            set
            {
                Debug.Assert(Mock?.Object != null);
                Mock.Object.IsActive = value;
            }
        }

        public Guid Identifier
        {
            get
            {
                Debug.Assert(Mock?.Object != null);
                return Mock.Object.Identifier;
            }
        }

        public void Initialize(bool isEnabled)
        {
            Debug.Assert(Mock?.Object != null);
            Mock.Object.Initialize(isEnabled);
        }
    }
}
