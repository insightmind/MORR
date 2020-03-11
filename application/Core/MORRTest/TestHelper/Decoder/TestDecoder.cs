using System.Diagnostics;
using System.Threading;
using Moq;
using MORR.Core.Data.Transcoding;
using MORR.Shared.Utility;

namespace MORRTest.TestHelper.Decoder
{
    public class TestDecoder: IDecoder
    {
        public readonly Mock<IDecoder> Mock = new Mock<IDecoder>();

        public ManualResetEvent DecodeFinished => Mock?.Object?.DecodeFinished;

        public void Decode(DirectoryPath recordingDirectoryPath)
        {
            Debug.Assert(Mock?.Object != null);
            Mock.Object.Decode(recordingDirectoryPath);
        }
    }
}
