namespace MORR.Core
{
    /// <summary>
    ///     Responsible for bootstrapping the application and providing compositional facilities.
    /// </summary>
    public interface IBootstrapper
    {
        /// <summary>
        ///     Composes the provided objects.
        /// </summary>
        /// <param name="object">The object to compose.</param>
        void ComposeImports(object @object);
    }
}