namespace SFTPManager.ViewModels
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Notification.Wpf;
    using SFTPManager.Models;
    using SFTPManager.Services;

    public class AddConnectionViewModel : ObservableObject
    {
        private string testResult;
        private SftpSettings settings = new SftpSettings();
        private readonly SessionService sessionService;
        private readonly NotificationService notificationService;
        private bool isUpdating;

        public event Action<SftpSettings> ConnectionSaved;

        public AddConnectionViewModel()
        {
            sessionService = new SessionService();
            notificationService = new NotificationService();

            TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);
            DisconnectCommand = new RelayCommand(Disconnect);
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

        public bool IsUpdating
        {
            get => isUpdating;
            set => SetProperty(ref isUpdating, value);
        }

        public ICommand TestConnectionCommand { get; }

        public ICommand DisconnectCommand { get; }

        private async Task TestConnectionAsync()
        {
            if (IsInputValid())
            {
                try
                {
                    TestResult = await SftpService.Instance.ConnectAsync(Settings);
                    SaveConnection();
                    ShowNotification("Connection", "Connection successful.", NotificationType.Success);
                }
                catch (Exception ex)
                {
                    HandleConnectionError(ex);
                }
            }
        }

        private bool IsInputValid()
        {
            if (string.IsNullOrEmpty(Settings.Server) || string.IsNullOrEmpty(Settings.Username))
            {
                ShowNotification("Validation Error", "Please fill in both server and username fields.", NotificationType.Warning);
                return false;
            }

            Match matchIP = Regex.Match(Settings.Server, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$", RegexOptions.IgnoreCase);
            Match matchHN = Regex.Match(Settings.Server, @"^(([a-zA-Z]|[a-zA-Z][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z]|[A-Za-z][A-Za-z0-9\-]*[A-Za-z0-9])$", RegexOptions.IgnoreCase);

            if (!matchIP.Success && !matchHN.Success)
            {
                ShowNotification("Validation Error", "The hostname or IP address you have entered appears to be invalid.", NotificationType.Warning);
                return false;
            }

            return true;
        }

        private void SaveConnection()
        {
            var connections = sessionService.LoadData();

            if (IsUpdating)
            {
                UpdateExistingConnection(connections);
            }
            else
            {
                connections.Add(this.Settings);
            }

            sessionService.SaveData(connections);
            ConnectionSaved?.Invoke(Settings); // Вызов события
        }

        private void UpdateExistingConnection(ICollection<SftpSettings> connections)
        {
            var existingConnection = connections.FirstOrDefault(c => c.Server == Settings.Server && c.Username == Settings.Username);
            if (existingConnection != null)
            {
                existingConnection.Port = Settings.Port;
                existingConnection.Password = Settings.Password;
            }
        }

        private void Disconnect()
        {
            try
            {
                SftpService.Instance.Disconnect();
                TestResult = "Server disconnected";
                ShowNotification("Disconnected", "Successfully disconnected from the server.", NotificationType.Information);
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
