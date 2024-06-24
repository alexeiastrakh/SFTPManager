namespace SFTPManager.Services
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Xml.Linq;
    using SFTPManager.Models;

    public class SessionService
    {
        private const string DataFilePath = "data.xml";
        private XDocument xmlDoc;

        public SessionService()
        {
            EnsureDataFileExists();
            xmlDoc = XDocument.Load(DataFilePath);
        }

        private void EnsureDataFileExists()
        {
            if (!System.IO.File.Exists(DataFilePath))
            {
                new XDocument(new XElement("data", new XElement("sessions"))).Save(DataFilePath);
            }
        }

        public ObservableCollection<SftpSettings> LoadData()
        {
            var connections = new ObservableCollection<SftpSettings>();
            var sessions = xmlDoc.Descendants("session").Select(session => new SftpSettings
            {
                Server = session.Element("server").Value,
                Port = int.Parse(session.Element("port").Value),
                Username = session.Element("username").Value
            });
            foreach (var sftpSettings in sessions)
            {
                connections.Add(sftpSettings);
            }
            return connections;
        }

        public void SaveData(ObservableCollection<SftpSettings> connections)
        {
            var sessionsNode = xmlDoc.Element("data").Element("sessions");
            sessionsNode.RemoveAll();

            foreach (var connection in connections)
            {
                sessionsNode.Add(new XElement("session",
                    new XElement("server", connection.Server),
                    new XElement("port", connection.Port),
                    new XElement("username", connection.Username)));
            }

            xmlDoc.Save(DataFilePath);
        }
    }
}
