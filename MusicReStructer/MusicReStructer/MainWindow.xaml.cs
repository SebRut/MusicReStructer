using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Windows.Forms;

namespace MusicReStructer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MusicDirectory musicDirectory;

// ReSharper disable once CSharpWarnings::CS1591
        public MainWindow()
        {
            InitializeComponent();
            musicDirectory = null;
        }


        private void PickDirectoryCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {  
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                musicDirectory = new MusicDirectory(folderBrowserDialog.SelectedPath, RecursiveSwitch.IsChecked.Value);
                BaseDirectoryLabel.Content = musicDirectory.Configuration.BaseDirectory;
                MRSDirectoryLabel.Content = musicDirectory.Configuration.TempDirectory;
            }

        }

        private void StartCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (musicDirectory != null) e.CanExecute = Directory.Exists(musicDirectory.Configuration.BaseDirectory);
            else e.CanExecute = false;
        }

        private void StartCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            musicDirectory.StartProcessing();
            StartButton.Visibility = Visibility.Collapsed;
            StopButton.Visibility = Visibility.Visible;
        }

        private void StopCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //TODO Implement Stop
        }
    }
}
