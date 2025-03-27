using System.Windows;
using System.Windows.Controls;
using CollectionManager.Core;
using Microsoft.Win32;

namespace CollectionManager
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            DestinationTextBox.Text = Config.DestinationFolder;
            ThreadsInput.Value = Config.Threads;
            OsuPathTextbox.Text = Config.OsuPath;
        }

        private void SelectDestinationFolder(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select destination folder",
                InitialDirectory = Config.DestinationFolder
            };
            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                Config.DestinationFolder = folderName;
                Config.Save();
                DestinationTextBox.Text = Config.DestinationFolder;
            }
        }

        private void ChangeThreads(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            Config.Threads = ThreadsInput.Value;
            Config.Save();
        }

        private void SelectOsuPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select destination folder",
                InitialDirectory = Config.OsuPath
            };
            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                Config.OsuPath = folderName;
                Config.Save();
                OsuPathTextbox.Text = Config.OsuPath;
            }
        }
    }
}
