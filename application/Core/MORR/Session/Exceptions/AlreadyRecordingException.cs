﻿namespace MORR.Core.Session.Exceptions
{
    /// <summary>
    ///     Exception thrown if a recording is started while another recording is already running.
    /// </summary>
    public class AlreadyRecordingException : RecordingException { }
}