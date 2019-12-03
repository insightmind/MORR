using System.Windows;

namespace Morr.Core.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ErrorDialog.xaml
    /// </summary>
    public partial class ErrorDialog : Window
    {
        public ErrorDialog(string errorMessage)
        {
            ErrorMessage = errorMessage;
            InitializeComponent();
            DataContext = this;
        }

        public string ErrorMessage { get; }
    }
}