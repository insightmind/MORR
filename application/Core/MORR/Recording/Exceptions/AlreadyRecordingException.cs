namespace MORR.Core.Recording.Exceptions
{
    /// <summary>
    ///     Exception thrown if a recording is started while another recording is already running.
    /// </summary>
    public class AlreadyRecordingException : RecordingException { }
}