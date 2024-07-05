namespace SFTPManager.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Threading.Tasks;
    using Notification.Wpf;
    using Renci.SshNet;
    using SFTPManager.Models;
    using SFTPManager.Resources;

    public sealed class SftpService : IDisposable
    {
        private static readonly Lazy<SftpService> lazyInstance = new Lazy<SftpService>(() => new SftpService());
        private SftpClient sftpClient;
        private bool isConnected;


        private SftpService() { }

        public static SftpService Instance => lazyInstance.Value;

        public async Task<string> ConnectAsync(SftpSettings settings)
        {
            if (isConnected)
            {
                throw new InvalidOperationException("Already connected to SFTP server");
            }

            sftpClient = new SftpClient(settings.Server, settings.Port, settings.Username, settings.Password);

            await Task.Run(() => sftpClient.Connect());

            isConnected = sftpClient.IsConnected;

            return isConnected ? Resources_en.ConnectionSuccessful : Resources_en.ConnectionUnsuccessful;
        }
        public void Disconnect()
        {
            if (isConnected)
            {
                sftpClient.Disconnect();
                sftpClient.Dispose();
                isConnected = false;
            }
            else
            {
                throw new InvalidOperationException("Not connected to SFTP server");
            }
        }
        public async Task UploadFileAsync(string localFilePath, string remoteFilePath)
        {
            EnsureConnected();

            string remoteDirectory = Path.GetDirectoryName(remoteFilePath);
            if (!string.IsNullOrEmpty(remoteDirectory) && !DirectoryExists(remoteDirectory))
            {
                CreateDirectory(remoteDirectory);
            }

            using var fileStream = new FileStream(localFilePath, FileMode.Open);
            await Task.Run(() => sftpClient.UploadFile(fileStream, remoteFilePath));
        }

        private bool DirectoryExists(string directoryPath)
        {
            try
            {
                sftpClient.ChangeDirectory(directoryPath);
                sftpClient.ChangeDirectory("/");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void CreateDirectory(string directoryPath)
        {
            sftpClient.CreateDirectory(directoryPath);
        }

        public async Task DownloadFileAsync(string remoteFilePath, string localFilePath)
        {
            EnsureConnected();

            string localFileName = Path.Combine(localFilePath, Path.GetFileName(remoteFilePath));
            using Stream stream = File.OpenWrite(localFileName);
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