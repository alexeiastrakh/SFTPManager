namespace SFTPManager.Models
{
    using System.Collections.ObjectModel;

    public class ServerDetail
    {
        public ObservableCollection<SftpSettings> Connections { get; set; } = new ObservableCollection<SftpSettings>();

    }
}
