namespace SFTPManager.Models
{
    using System;

    public class ServerFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime FileCreationTime { get; set; }
    }
}
