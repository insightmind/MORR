namespace MORR.Core.CLI.Commands
{
    public interface ICommand<in TOptions> where TOptions : CommandOptions
    {
        int Execute(TOptions options);
    }
}
