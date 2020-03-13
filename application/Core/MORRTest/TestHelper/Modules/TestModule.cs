using System;
using System.Diagnostics;
using Moq;
using MORR.Shared.Modules;

namespace MORRTest.TestHelper.Modules
{
    /// <summary>
    /// The TestModule class encapsulates a IModule mock.
    /// The Purpose for this class is to inject a mock into a given configuration.
    /// As this requires discovering the module via its class name we
    /// cannot simply inject the Mock Object itself and therefore need to wrap it using
    /// this class.
    ///
    /// Finally this results into a class which simply propagates the values to the mock object.
    /// </summary>
    public class TestModule : IModule
    {
        /// <summary>
        /// The Mock which can be used to verify calls using Moq
        /// </summary>
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
