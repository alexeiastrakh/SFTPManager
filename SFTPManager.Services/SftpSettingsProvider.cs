using SFTPManager.Core.Interfaces;
using SFTPManager.Models;

namespace SFTPManager.Services
{
    public class SftpSettingsProvider : ISftpSettingsProvider
    {
        private static SftpSettingsProvider instance;
        private SftpSettings settings;

        private SftpSettingsProvider()
        {
            settings = new SftpSettings(); // Initialize settings
        }

        public static SftpSettingsProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SftpSettingsProvider();
                }
                return instance;
            }
        }

        public SftpSettings GetSettings()
        {
            return settings;
        }

        public void SetSettings(SftpSettings settings)
        {
            // Ensure settings is initialized
            if (this.settings == null)
            {
                this.settings = new SftpSettings();
            }

            // Update all properties of settings
            this.settings.Server = settings.Server;
            this.settings.Username = settings.Username;
            this.settings.Password = settings.Password;
            this.settings.LocalPath = settings.LocalPath;
            this.settings.RemotePath = settings.RemotePath;
            this.settings.Port = settings.Port;
        }
    }
}
