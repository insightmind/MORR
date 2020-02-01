namespace MORR.Core.CLI.Commands
{
    interface ICommand<in TOptions> where TOptions : CommandOptions
    {
        int Execute(TOptions options);
    }
}
