using Windows.UI.Xaml.Input;
using CommandLine;

namespace Morr.Core.CLI.Commands
{
    public interface ICLICommand<in TOptions> where TOptions : CommandOptions
    {
        int Execute(TOptions options);
    }
}
