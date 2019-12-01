using System.Windows.Controls;
using MORR.Core.UI.ViewModels;

namespace MORR.Core.UI
{
    /// <summary>
    ///     Interaction logic for Bootstrapper.xaml
    /// </summary>
    public partial class Bootstrapper : UserControl
    {
        public Bootstrapper()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ApplicationViewModel ViewModel { get; } = new ApplicationViewModel();
    }
}