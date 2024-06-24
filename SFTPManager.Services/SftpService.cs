namespace SFTPManager.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Notification.Wpf;
    using Renci.SshNet;
    using SFTPManager.Models;

    public sealed class SftpService : IDisposable
    {
        private static readonly Lazy<SftpService> lazyInstance = new Lazy<SftpService>(() => new SftpService());
        private readonly NotificationService notificationService = new NotificationService();
        private SftpClient sftpClient;
        private SshClient sshClient;
        private bool isConnected;

        private SftpService() { }

        public static SftpService Instance => lazyInstance.Value;

        public async Task<string> ConnectAsync(SftpSettings settings)
        {
            if (isConnected)
            {
                throw new InvalidOperationException("Already connected");
            }

            sftpClient = new SftpClient(settings.Server, settings.Port, settings.Username, settings.Password);
            sshClient = new SshClient(settings.Server, settings.Port, settings.Username, settings.Password);

            await Task.WhenAll(Task.Run(() => sftpClient.Connect()), Task.Run(() => sshClient.Connect()));
            isConnected = sftpClient.IsConnected && sshClient.IsConnected;

            return isConnected ? "Connection successful" : "Connection failed";
        }

        public void Disconnect()
        {
            if (isConnected)
            {
                sftpClient.Disconnect();
                sshClient.Disconnect();
                sftpClient.Dispose();
                sshClient.Dispose();
                isConnected = false;
            }
            else
            {
                throw new InvalidOperationException("Not connected to SFTP server");
            }
        }

        public SshCommand CreateCommand(string commandText)
        {
            if (!isConnected)
            {
                notificationService.ShowNotification("Warning", "Not connected to SFTP server", NotificationType.Warning);
                return null;
            }

            return sshClient.CreateCommand(commandText);
        }

        public async Task UploadFileAsync(string localFilePath, string remoteFilePath)
        {
            EnsureConnected();

            string fileName = Path.GetFileName(localFilePath);
            using var fileStream = new FileStream(localFilePath, FileMode.Open);
            await Task.Run(() => sftpClient.UploadFile(fileStream, Path.Combine(remoteFilePath, fileName)));
        }

        public async Task DownloadFileAsync(string remoteFilePath, string localFilePath)
        {
            EnsureConnected();

            using Stream stream = File.OpenWrite(localFilePath + Path.GetFileName(remoteFilePath));
            await Task.Run(() => sftpClient.DownloadFile(remoteFilePath, stream));
        }

        public async Task<IEnumerable<string>> ListRemoteFilesAsync(string remoteDirectory)
        {
            EnsureConnected();

            var files = await Task.Run(() => sftpClient.ListDirectory(remoteDirectory));
            return files.Where(file => !file.Name.StartsWith(".")).Select(file => file.FullName);
        }

        public async Task<List<FileDetails>> ListRemoteFileSystemItemsAsync(string remotePath)
        {
            EnsureConnected();

            var files = await Task.Run(() => sftpClient.ListDirectory(remotePath));
            var items = new List<FileDetails>();

            foreach (var file in files.Where(file => file.Name != "." && file.Name != ".."))
            {
                var item = new FileDetails
                {
                    Name = file.Name,
                    IsDirectory = file.IsDirectory
                };

                if (file.IsDirectory)
                {
                    item.Children = new ObservableCollection<FileDetails>(await ListRemoteFileSystemItemsAsync(file.FullName));
                }

                items.Add(item);
            }

            return items;
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void EnsureConnected()
        {
            if (!isConnected)
            {
                throw new InvalidOperationException("Not connected to SFTP server");
            }
        }
    }
}
