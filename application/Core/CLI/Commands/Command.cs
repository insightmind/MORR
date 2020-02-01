namespace MORR.Core.CLI.Commands
{
    interface ICommand<in TOptions> where TOptions : CommandOptions
    {
        public int Execute(TOptions options);
    }
}
