namespace MORR.Core.CLI.Commands
{
    internal interface ICommand<in TOptions> where TOptions : CommandOptions
    {
        int Execute(TOptions options);
    }
}
