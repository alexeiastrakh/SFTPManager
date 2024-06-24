namespace SFTPManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.ComponentModel;

    public class FileDetails : ObservableObject
    {
        public string Name { get; set; }

        public bool IsDirectory { get; set; }

        public ObservableCollection<FileDetails> Children { get; set; }

        public FileDetails()
        {
            Children = new ObservableCollection<FileDetails>();
        }
    }
}
