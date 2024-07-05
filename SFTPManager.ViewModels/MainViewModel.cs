namespace SFTPManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SFTPManager.Models;

    public class MainViewModel : ObservableObject
    {
        private ObservableObject currentViewModel;
        private ObservableCollection<ObservableObject> viewModels;
        private ObservableCollection<SftpSettings> connections;
        private SftpSettings selectedConnection;

        public MainViewModel()
        {
            ViewModels = new ObservableCollection<ObservableObject>
            {
                new ServerDetailsViewModel(),
                new TerminalViewModel(),
                new FileTransferViewModel(),
                new SettingsViewModel(),
                new AddConnectionViewModel()
            };

            CurrentViewModel = ViewModels[0];

            Connections = new ObservableCollection<SftpSettings>();

            ServersBtnClickCommand = new RelayCommand(() => CurrentViewModel = ViewModels[0]);
            TerminalBtnClickCommand = new RelayCommand(() => CurrentViewModel = ViewModels[1]);
            FileTransferBtnClickCommand = new RelayCommand(() => CurrentViewModel = ViewModels[2]);
            SettingsBtnClickCommand = new RelayCommand(() => CurrentViewModel = ViewModels[3]);
            OpenAddConnectionCommand = new RelayCommand(() => CurrentViewModel = ViewModels[4]);
        }

        public ObservableObject CurrentViewModel
        {
            get => currentViewModel;
            set => SetProperty(ref currentViewModel, value);
        }

        public ObservableCollection<ObservableObject> ViewModels
        {
            get => viewModels;
            set => SetProperty(ref viewModels, value);
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

        public ICommand ServersBtnClickCommand { get; }
        public ICommand TerminalBtnClickCommand { get; }
        public ICommand FileTransferBtnClickCommand { get; }
        public ICommand SettingsBtnClickCommand { get; }
        public ICommand OpenAddConnectionCommand { get; }
    }
}
