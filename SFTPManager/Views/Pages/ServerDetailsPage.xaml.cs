using System.Windows.Controls;
using System.Windows;
using SFTPManager.ViewModels;

namespace SFTPManager.Views.Pages
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
            if (this.DataContext is ServerDetailsViewModel viewModel)
            {
                viewModel.SubscribeToConnectionSavedEvent();
            }
        }
    }
}
