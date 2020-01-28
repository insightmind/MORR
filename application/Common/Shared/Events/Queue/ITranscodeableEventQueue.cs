namespace MORR.Shared.Events.Queue
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for <see cref="Event" /> types intended for transcoding.
    /// </summary>
    /// <typeparam name="T">The type of the event</typeparam>
    public interface ITranscodeableEventQueue<T> : IEncodeableEventQueue<T>, IDecodeableEventQueue<T>
        where T : Event { }
}