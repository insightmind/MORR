using System.Diagnostics;
using System.Threading;
using Moq;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Utility;

namespace MORRTest.TestHelper.Decoder
{
    /// <summary>
    /// The TestModule class encapsulates a IDecoder mock.
    /// The Purpose for this class is to inject a mock into a given configuration.
    /// As this requires discovering the decoder via its class name we
    /// cannot simply inject the Mock Object itself and therefore need to wrap it using
    /// this class.
    ///
    /// Finally this results into a class which simply propagates the values to the mock object.
    /// </summary>
    public class TestDecoder: IDecoder
    {
        /// <summary>
        /// The Mock which can be used to verify calls using Moq
        /// </summary>
        public readonly Mock<IDecoder> Mock = new Mock<IDecoder>();

        public ManualResetEvent DecodeFinished => Mock?.Object?.DecodeFinished;

        public void Decode(DirectoryPath recordingDirectoryPath)
        {
            Debug.Assert(Mock?.Object != null);
            Mock.Object.Decode(recordingDirectoryPath);
        }
    }
}
