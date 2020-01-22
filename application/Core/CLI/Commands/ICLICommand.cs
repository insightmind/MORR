namespace Morr.Core.CLI.Commands
{
    public interface ICLICommand<in TOptions> where TOptions : ICommandOptions
    {
        int Execute(TOptions options);
    }
}
