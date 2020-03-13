using System.Diagnostics;
using System.Threading;
using Moq;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Utility;

namespace MORRTest.TestHelper.Encoder
{
    /// <summary>
    /// The TestEncoder class encapsulates a IEncoder mock.
    /// The Purpose for this class is to inject a mock into a given configuration.
    /// As this requires discovering the encoder via its class name we
    /// cannot simply inject the Mock Object itself and therefore need to wrap it using
    /// this class.
    ///
    /// Finally this results into a class which simply propagates the values to the mock object.
    /// </summary>
    public class TestEncoder : IEncoder
    {
        /// <summary>
        /// The Mock which can be used to verify calls using Moq
        /// </summary>
        public Mock<IEncoder> Mock = new Mock<IEncoder>();

        public ManualResetEvent EncodeFinished => Mock?.Object?.EncodeFinished;

        public void Encode(DirectoryPath recordingDirectoryPath)
        {
            Debug.Assert(Mock?.Object != null);
            Mock.Object.Encode(recordingDirectoryPath);
        }
    }
}
