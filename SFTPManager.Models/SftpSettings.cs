namespace SFTPManager.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;

    public class SftpSettings : ObservableObject
    {
        private string server;
        private string username;
        private string password;
        private string localPath;
        private string remotePath;

        private const int DefaultPort = 22;
        private int port = DefaultPort;

        public int Port
        {
            get => port;
            set
            {
                SetProperty(ref port, value);
            }
        }

        public string Server
        {
            get => server;
            set
            {
                SetProperty(ref server, value);
            }
        }

        public string Username
        {
            get => username;
            set
            {
                SetProperty(ref username, value);
            }
        }

        public string Password
        {
            get => password;
            set
            {
                SetProperty(ref password, value);
            }
        }

        public string LocalPath
        {
            get => localPath;
            set => SetProperty(ref localPath, value);
        }

        public string RemotePath
        {
            get => remotePath;
            set => SetProperty(ref remotePath, value);
        }
    }
}
