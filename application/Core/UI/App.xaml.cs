using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MORR.Core.UI
{
    /// <summary>
    ///     Entry point for WPF application
    /// </summary>
    public partial class App : Application
    {
        internal static bool HasLanguageCommandLineArgument { get; private set; }

        internal static bool IsLanguageCommandLineArgument(string argument)
        {
            return string.CompareOrdinal(argument, "no-force-german-ui") == 0;
        }

        public App()
        {
            // Set culture before WPF components get initialized so they use the correct language

            HasLanguageCommandLineArgument = Environment.GetCommandLineArgs().Any(IsLanguageCommandLineArgument);

            if (!HasLanguageCommandLineArgument)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
            }
        }
    }
}