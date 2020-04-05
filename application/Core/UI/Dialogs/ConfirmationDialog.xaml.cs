using System.Windows;

namespace MORR.Core.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for ConfirmationDialog.xaml
    /// </summary>
    public partial class ConfirmationDialog : Window
    {
        public ConfirmationDialog()
        {
            InitializeComponent();
        }

        private void OnAccept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}