namespace Morr.Core.CLI.Commands
{
    internal interface ICLICommand<in TOptions> where TOptions : ICommandOptions
    {
        int Execute(TOptions options);
    }
}
