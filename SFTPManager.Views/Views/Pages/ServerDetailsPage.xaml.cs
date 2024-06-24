using SFTPManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace TestAssignmentForDCT.Views.Pages
{
    public partial class ServerDetailsPage : Page
    {
        public ServerDetailsPage()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ServerDetailsViewModel viewModel)
            {
                viewModel.NavigateToAddConnectionCommand.Execute(null);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ServerDetailsViewModel;
            viewModel?.LoadData();
        }
    }
}
