using System.Windows.Controls;
using System.Windows.Input;
using SFTPManager.ViewModels;

namespace SFTPManager.Views.Pages
{
    public partial class TerminalPage : Page
    {
        public TerminalPage()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is TerminalViewModel viewModel)
                {
                    viewModel.SendCommandToServerAsync();
                }
                e.Handled = true; 
            }
        }

    }
}
