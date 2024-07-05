namespace SFTPManager.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.Win32;
    using Notification.Wpf;
    using SFTPManager.Models;
    using SFTPManager.Resources;
    using SFTPManager.Services;

    public class FileTransferViewModel : ObservableObject
    {
        private SftpSettings settings;
        private string result;
        private ObservableCollection<FileDetails> remoteFileSystemItems;

        public FileTransferViewModel()
        {
            settings = new SftpSettings();
            RemoteFileSystemItems = new ObservableCollection<FileDetails>();
            UploadCommand = new AsyncRelayCommand(UploadFileAsync);
            DownloadCommand = new AsyncRelayCommand(DownloadFileAsync);
            OpenFileDialogCommand = new RelayCommand(OpenFileDialog);
            LoadRemoteFileSystemCommand = new AsyncRelayCommand(LoadRemoteFileSystemAsync);
        }

        public FileTransferViewModel(SftpSettings settings) : this()
        {
            this.settings = settings;
        }

        public SftpSettings Settings
        {
            get => settings;
            set => SetProperty(ref settings, value);
        }

        public string Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        public ObservableCollection<FileDetails> RemoteFileSystemItems
        {
            get => remoteFileSystemItems;
            set => SetProperty(ref remoteFileSystemItems, value);
        }

        public ICommand UploadCommand { get; }

        public ICommand DownloadCommand { get; }

        public ICommand OpenFileDialogCommand { get; }

        public ICommand LoadRemoteFileSystemCommand { get; }

        private void ShowNotification(string title, string message, NotificationType type = NotificationType.Information)
        {
            var notificationManager = new NotificationManager();
            notificationManager.Show(title, message, type, "", TimeSpan.FromSeconds(5));
        }

        private async Task UploadFileAsync()
        {
            if (!ArePathsValid())
            {
                ShowNotification(Resources_en.ErrorTitle, Resources_en.ErrorSelectPaths, NotificationType.Warning);
                return;
            }

            try
            {
                string remoteFilePath;

                if (string.IsNullOrEmpty(Settings.RemotePath))
                {
                    remoteFilePath = Path.GetFileName(Settings.LocalPath);
                }
                else
                {
                    remoteFilePath = Settings.RemotePath.EndsWith("/") ? $"{Settings.RemotePath}{Path.GetFileName(Settings.LocalPath)}" : $"{Settings.RemotePath}/{Path.GetFileName(Settings.LocalPath)}";
                }

                await SftpService.Instance.UploadFileAsync(Settings.LocalPath, remoteFilePath);
                Result = $"{Resources_en.UploadSuccess} Name: {Path.GetFileName(Settings.LocalPath)}";
                ShowNotification("File Upload", Resources_en.UploadSuccess);
            }
            catch (Exception ex)
            {
                HandleError(ex, Resources_en.FileUploadFailed);
            }
        }

        private async Task DownloadFileAsync()
        {
            if (!ArePathsValid())
            {
                ShowNotification(Resources_en.ErrorTitle, Resources_en.ErrorSelectPaths, NotificationType.Warning);
                return;
            }

            try
            {
                if (File.Exists(Settings.LocalPath))
                {
                    ShowNotification("File Download", Resources_en.FileExists);
                }

                string remoteFilePath = Settings.RemotePath.Replace("\\", "/");
                string localFilePath = Settings.LocalPath;
                await SftpService.Instance.DownloadFileAsync(remoteFilePath, localFilePath);
                Result = $"{Resources_en.DownloadSuccess} Name: {Path.GetFileName(Settings.LocalPath)}";
                ShowNotification("File Download", Resources_en.DownloadSuccess);
            }
            catch (UnauthorizedAccessException uex)
            {
                HandleError(uex, Resources_en.AccessDenied);
            }
            catch (Exception ex)
            {
                HandleError(ex, Resources_en.FileDownloadFailed);
            }
        }

        private void OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Settings.LocalPath = openFileDialog.FileName;
            }
        }

        private async Task LoadRemoteFileSystemAsync()
        {
            try
            {
                var items = await SftpService.Instance.ListRemoteFileSystemItemsAsync("/");
                RemoteFileSystemItems.Clear();
                foreach (var item in items)
                {
                    RemoteFileSystemItems.Add(item);
                }

                Result = Resources_en.FileSystemLoaded;
            }
            catch (Exception ex)
            {
                HandleError(ex, "Loading File System Failed");
            }
        }

        private bool ArePathsValid()
        {
            return !string.IsNullOrEmpty(Settings.LocalPath);
        }

        private void HandleError(Exception ex, string title)
        {
            Result = ex.Message;
            ShowNotification(title, ex.Message, NotificationType.Error);
        }
    }
}
