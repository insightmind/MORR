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
        public App()
        {
            // Set culture before WPF components get initialized so they use the correct language

            var forceGerman = Environment.GetCommandLineArgs()
                                         .All(x => string.CompareOrdinal(x, "no-force-german-ui") != 0);

            if (forceGerman)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
            }
        }
    }
}