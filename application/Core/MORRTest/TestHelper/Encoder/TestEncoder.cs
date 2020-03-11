using System.Diagnostics;
using System.Threading;
using Moq;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Utility;

namespace MORRTest.TestHelper.Encoder
{
    public class TestEncoder : IEncoder
    {
        public Mock<IEncoder> Mock = new Mock<IEncoder>();

        public ManualResetEvent EncodeFinished => Mock?.Object?.EncodeFinished;

        public void Encode(DirectoryPath recordingDirectoryPath)
        {
            Debug.Assert(Mock?.Object != null);
            Mock.Object.Encode(recordingDirectoryPath);
        }
    }
}
