using System.Windows;

namespace MORR.Core.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for InformationDialog.xaml
    /// </summary>
    public partial class InformationDialog : Window
    {
        public InformationDialog()
        {
            InitializeComponent();
        }

        private void OnAccept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}