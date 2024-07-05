namespace SFTPManager.ViewModels
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Notification.Wpf;
    using SFTPManager.Core;
    using SFTPManager.Core.Interfaces;
    using SFTPManager.Helpers;
    using SFTPManager.Models;
    using SFTPManager.Resources;
    using SFTPManager.Services;

    public class AddConnectionViewModel : ObservableObject
    {
        private SftpSettings settings;
        private string testResult;
        private readonly NotificationService notificationService;
        private ISftpSettingsProvider settingsProvider;
        private SshService sshService;
        private readonly SessionService sessionService;

        public event Action<SftpSettings> ConnectionSaved;

        public AddConnectionViewModel()
        {
            notificationService = new NotificationService();
            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);
            DisconnectCommand = new RelayCommand(Disconnect);
            settingsProvider = SftpSettingsProvider.Instance;
            sessionService = new SessionService();
            Settings = new SftpSettings();
        }

        public SftpSettings Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        public string TestResult
        {
            get => testResult;
            set => SetProperty(ref testResult, value);
        }

        public bool IsUpdating { get; set; }

        public ICommand TestConnectionCommand { get; }

        public ICommand DisconnectCommand { get; }

        private async Task TestConnectionAsync()
        {
            if (IsInputValid())
            {
                try
                {
                    if (Settings != null)
                    {
                        TestResult = await SftpService.Instance.ConnectAsync(Settings);
                        if (TestResult == Resources_en.ConnectionSuccessful)
                        {
                            SaveConnection();
                            ShowNotification("Connection", Resources_en.ConnectionSuccessful, NotificationType.Success);
                        }
                    }
                    else
                    {
                        ShowNotification("Connection Error", "Settings or service not available.", NotificationType.Error);
                    }
                }
                catch (Exception ex)
                {
                    HandleConnectionError(ex);
                }
            }
        }

        private bool IsInputValid()
        {
            if (Settings == null || string.IsNullOrEmpty(Settings.Server) || string.IsNullOrEmpty(Settings.Username))
            {
                ShowNotification("Validation Error", Resources_en.ValidationServerUsernameEmpty, NotificationType.Warning);
                return false;
            }

            if (!IsValidIpAddress(Settings.Server) && !HostnameValidation.Validate(Settings.Server))
            {
                ShowNotification("Validation Error", Resources_en.ValidationInvalidHostOrIp, NotificationType.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidIpAddress(string address)
        {
            return IPAddress.TryParse(address, out _);
        }

        private void SaveConnection()
        {
            var connections = sessionService.LoadData();

            var existingConnection = connections.FirstOrDefault(c => c.Server == Settings.Server && c.Username == Settings.Username);
            if (existingConnection != null)
            {
                existingConnection.Port = Settings.Port;
                existingConnection.Password = Settings.Password;
            }
            else
            {
                connections.Add(Settings);
            }

            sessionService.SaveData(connections);
            ConnectionSaved?.Invoke(Settings);
            settingsProvider.SetSettings(Settings);
        }

        private void Disconnect()
        {
            try
            {
                SftpService.Instance.Disconnect();
                TestResult = Resources_en.DisconnectedSuccessfully;
                ShowNotification("Disconnected", Resources_en.DisconnectedSuccessfully, NotificationType.Information);
            }
            catch (Exception ex)
            {
                HandleConnectionError(ex);
            }
        }

        private void ShowNotification(string title, string message, NotificationType type)
        {
            notificationService.ShowNotification(title, message, type);
        }

        private void HandleConnectionError(Exception ex)
        {
            TestResult = ex.Message;
            ShowNotification("Connection Failed", ex.Message, NotificationType.Error);
        }
    }
}
