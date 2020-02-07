namespace MORR.Core.CLI.Commands
{
    /// <summary>
    /// Simple interface to implement the command pattern for the CLI.
    /// </summary>
    /// <typeparam name="TOptions">The options which are to be met for execution of the command.</typeparam>
    public interface ICommand<in TOptions> where TOptions : CommandOptions
    {
        /// <summary>
        /// Executes the command with the given options.
        /// </summary>
        /// <param name="options">The resolved options used to customize the command.</param>
        /// <returns>Return code: 0 if successful, -1 on failure. Custom return codes are allowed.</returns>
        int Execute(TOptions options);
    }
}
