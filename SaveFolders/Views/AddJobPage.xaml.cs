using Ookii.Dialogs.Wpf;
using SaveFolders.Helpers;
using SaveFolders.Models;
using SaveFolders.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace SaveFolders.Views
{
    /// <summary>
    /// Logique d'interaction pour AddJobPage.xaml
    /// </summary>
    public partial class AddJobPage : UserControl
    {
        private readonly MainWindow _mainWindow;
        private SaveJobRepository _repository = new();
        public AddJobPage(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            string? driveLetter = GetSelectedDriveLetter();
            if (string.IsNullOrEmpty(driveLetter))
            {
                MessageBox.Show("Veuillez sélectionner un disque.");
                return;
            }
            var serial = DriveHelper.GetVolumeSerial(driveLetter+":");

            if (string.IsNullOrWhiteSpace(serial))
            {
                MessageBox.Show("Impossible de récupérer le serial du disque.");
                return;
            }

            if (string.IsNullOrWhiteSpace(SourcePathTextBox.Text))
            {
                MessageBox.Show("Veuiller entre un chemin source");
                return;
            }
            if (string.IsNullOrWhiteSpace(DestinationFolderNameTextBox.Text))
            {
                MessageBox.Show("Veuiller entre un nom de dossier");
                return;
            }


            var job = new SaveJob
            {
                SourcePath = SourcePathTextBox.Text.Trim(),
                DiskSerial = serial.Trim(),
                DestinationFolderName = DestinationFolderNameTextBox.Text.Trim(),
            };

            List<SaveJob> SaveJobs = _repository.Load();

            SaveJobs.Add(job);
            _repository.Save(SaveJobs);
            _mainWindow.NavigateTo(0);
        }

        private string? GetSelectedDriveLetter()
        {
            if (DiskComboBox.SelectedItem is string selected)
            {
                // Exemple de selected = "C:\\ (Disque USB)"
                if (selected.Length > 0)
                {
                    // La lettre est le premier caractère (ex: 'C')
                    return selected.Substring(0, 1);
                }
            }
            return null;
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(0);
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            SourcePathTextBox.Text = dialog.SelectedPath;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var drives = DriveInfo.GetDrives()
                          .Where(d => d.IsReady)
                          .Select(d => $"{d.Name} ({d.VolumeLabel})")
                          .ToList();

            DiskComboBox.ItemsSource = drives;
        }
    }
}
