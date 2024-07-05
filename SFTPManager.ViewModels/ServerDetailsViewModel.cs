﻿namespace SFTPManager.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SFTPManager.Models;
    using SFTPManager.Services;

    public class ServerDetailsViewModel : ObservableObject
    {
        private ObservableCollection<SftpSettings> connections;
        private SftpSettings selectedConnection;
        private readonly SessionService sessionService;

        public ServerDetailsViewModel()
        {
            sessionService = new SessionService();
            Connections = new ObservableCollection<SftpSettings>();

            LoadData();
            SubscribeToConnectionSavedEvent();

            NavigateToAddConnectionCommand = new RelayCommand(NavigateToAddConnection, () => SelectedConnection != null);
            RemoveConnectionCommand = new RelayCommand<SftpSettings>(RemoveConnection, CanRemoveConnection);
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

        private void RemoveConnection(SftpSettings connection)
        {
            if (connection == null) return;

            Connections.Remove(connection);
            sessionService.SaveData(Connections);
            SelectedConnection = null;
        }

        private void NavigateToAddConnection()
        {
            if (Application.Current.MainWindow?.DataContext is MainViewModel mainViewModel)
            {
                var addConnectionViewModel = new AddConnectionViewModel();
                addConnectionViewModel.Settings = SelectedConnection;
                addConnectionViewModel.IsUpdating = true;
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
