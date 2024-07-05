namespace SFTPManager.Services
{
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Linq;
    using SFTPManager.Models;

    public class SessionService
    {
        private readonly string DataFilePath;

        public SessionService()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            DataFilePath = Path.Combine(appDataFolder, "data.json");

            EnsureDataFileExists();
        }

        private void EnsureDataFileExists()
        {
            if (!File.Exists(DataFilePath))
            {
                SaveData(new ObservableCollection<SftpSettings>());
            }
        }

        public ObservableCollection<SftpSettings> LoadData()
        {
            var json = File.ReadAllText(DataFilePath);
            var connections = JsonSerializer.Deserialize<ObservableCollection<SftpSettings>>(json);
            return connections ?? new ObservableCollection<SftpSettings>();
        }

        public void SaveData(ObservableCollection<SftpSettings> connections)
        {
            var json = JsonSerializer.Serialize(connections, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DataFilePath, json);
        }
    }
}
