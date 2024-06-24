using System.Windows.Controls;
using System.Windows.Input;
using TestAssignmentForDCT.ViewModels;

namespace TestAssignmentForDCT.Views.Pages
{
    public partial class TerminalPage : Page
    {
        public TerminalPage()
        {
            InitializeComponent();
        }

        private void CommandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = (TerminalViewModel)DataContext;
                if (viewModel.SendCommand.CanExecute(null))
                {
                    viewModel.SendCommand.Execute(null);
                }
            }
        }
    }
}
