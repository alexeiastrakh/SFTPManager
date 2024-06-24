using SFTPManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SFTPManager.ViewModels;

namespace SFTPManager.Views.Pages
{
    public partial class AddConnectionPage : Page
    {
        public AddConnectionPage()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddConnectionViewModel viewModel)
            {
                viewModel.Settings.Password = PasswordBox.Password;
            }
        }

    }
}
