using System.Windows;

namespace MORR.Core.UI.Dialogs
{
    /// <summary>
    ///     Interaction logic for SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : Window
    {
        public SaveDialog()
        {
            InitializeComponent();
        }

        private void OnAccept(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}