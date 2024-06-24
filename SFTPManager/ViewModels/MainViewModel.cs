namespace SFTPManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Controls;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SFTPManager.Models;
    using SFTPManager.Views.Pages;

    public class MainViewModel : ObservableObject
    {
        private Page currentPage;
        private Page serversPage;
        private Page terminalPage;
        private Page fileTransferPage;
        private Page settingsPage;
        private Page addConnectionPage;
        private ObservableCollection<SftpSettings> connections;
        private SftpSettings selectedConnection;

        public Page CurrentPage
        {
            get => currentPage;
            set => SetProperty(ref currentPage, value);
        }

        public ObservableCollection<SftpSettings> Connections
        {
            get => connections;
            set => SetProperty(ref connections, value);
        }

        public SftpSettings SelectedConnection
        {
            get => selectedConnection;
            set => SetProperty(ref selectedConnection, value);
        }

        public MainViewModel()
        {
            serversPage = new ServerDetailsPage();
            terminalPage = new TerminalPage();
            fileTransferPage = new FileTransferPage();
            settingsPage = new SettingsPage();
            addConnectionPage = new AddConnectionPage();

            CurrentPage = serversPage;

            connections = new ObservableCollection<SftpSettings>();

            ServersBtnClickCommand = new RelayCommand(() => CurrentPage = serversPage);
            TerminalBtnClickCommand = new RelayCommand(() => CurrentPage = terminalPage);
            FileTransferBtnClickCommand = new RelayCommand(() => CurrentPage = fileTransferPage);
            SettingsBtnClickCommand = new RelayCommand(() => CurrentPage = settingsPage);
            OpenAddConnectionCommand = new RelayCommand(() => CurrentPage = addConnectionPage);
        }

        public ICommand ServersBtnClickCommand { get; }

        public ICommand TerminalBtnClickCommand { get; }

        public ICommand FileTransferBtnClickCommand { get; }

        public ICommand SettingsBtnClickCommand { get; }

        public ICommand OpenAddConnectionCommand { get; }

    }
}
