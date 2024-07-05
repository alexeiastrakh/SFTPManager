namespace SFTPManager.Core.Interfaces
{
    using SFTPManager.Models;

    public interface ISftpSettingsProvider
    {
        SftpSettings GetSettings();
        void SetSettings(SftpSettings settings);
    }
}
