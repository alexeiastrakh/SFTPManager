namespace SFTPManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Notification.Wpf;
    using Renci.SshNet.Messages;
    using SFTPManager.Models;
    using SFTPManager.Resources;
    using SFTPManager.Services;

    public class ServerDetailsViewModel : ObservableObject
    {
        private ObservableCollection<SftpSettings> connections;
        private SftpSettings selectedConnection;
        private readonly NotificationService notificationService;
        private readonly SessionService sessionService;

        public ServerDetailsViewModel()
        {
            sessionService = new SessionService();
            Connections = new ObservableCollection<SftpSettings>();
            notificationService = new NotificationService();
            LoadData();
            SubscribeToConnectionSavedEvent();

            NavigateToAddConnectionCommand = new RelayCommand(NavigateToAddConnection, () => SelectedConnection != null);
            RemoveConnectionCommand = new RelayCommand<SftpSettings>(RemoveConnectionAsync, CanRemoveConnection);
        }

        public void SubscribeToConnectionSavedEvent()
        {
            if (Application.Current.MainWindow?.DataContext is MainViewModel mainViewModel)
            {
                if (mainViewModel.CurrentViewModel is AddConnectionViewModel addConnectionViewModel)
                {
                    addConnectionViewModel.ConnectionSaved += OnConnectionSaved;
                }
            }
        }

        public ObservableCollection<SftpSettings> Connections
        {
            get => connections;
            set => SetProperty(ref connections, value);
        }

        public SftpSettings SelectedConnection
        {
            get => selectedConnection;
            set
            {
                SetProperty(ref selectedConnection, value);
                UpdateCommandStates();
            }
        }

        public ICommand NavigateToAddConnectionCommand { get; }

        public ICommand RemoveConnectionCommand { get; }

        private void UpdateCommandStates()
        {
            ((RelayCommand)NavigateToAddConnectionCommand).NotifyCanExecuteChanged();
            ((RelayCommand<SftpSettings>)RemoveConnectionCommand).NotifyCanExecuteChanged();
        }

        private bool CanRemoveConnection(SftpSettings connection)
        {
            return connection != null;
        }

        private async void RemoveConnectionAsync(SftpSettings connection)
        {
            if (connection == null) return;

            if (await SftpService.Instance.IsConnectedAsync(connection))
            {
                SftpService.Instance.Disconnect();
            }

            Connections.Remove(connection);
            sessionService.SaveData(Connections);
            SelectedConnection = null;
            notificationService.ShowNotification("Connection removed", Resources_en.RemoveConnection, NotificationType.Success);
        }

        private void NavigateToAddConnection()
        {
            if (Application.Current.MainWindow?.DataContext is MainViewModel mainViewModel)
            {
                var addConnectionViewModel = new AddConnectionViewModel
                {
                    Settings = SelectedConnection,
                    IsUpdating = true
                };
                addConnectionViewModel.ConnectionSaved += OnConnectionSaved;

                mainViewModel.CurrentViewModel = addConnectionViewModel;
            }
        }

        private void OnConnectionSaved(SftpSettings updatedConnection)
        {
            var existingConnection = Connections.FirstOrDefault(c => c.Server == updatedConnection.Server && c.Username == updatedConnection.Username);
            if (existingConnection != null)
            {
                int index = Connections.IndexOf(existingConnection);
                Connections[index] = updatedConnection;
            }
            else
            {
                Connections.Add(updatedConnection);
            }

            sessionService.SaveData(Connections);
        }

        public void LoadData()
        {
            var loadedConnections = sessionService.LoadData();
            Connections.Clear();
            foreach (var connection in loadedConnections)
            {
                Connections.Add(connection);
            }
        }
    }
}
