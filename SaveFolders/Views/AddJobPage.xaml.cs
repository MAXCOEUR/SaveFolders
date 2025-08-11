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
        private SaveJob? _saveJob;
        private SaveJobRepository _repository = new();
        public AddJobPage(MainWindow mainWindow, SaveJob? saveJob = null)
        {
            InitializeComponent();

            _mainWindow = mainWindow;
            _saveJob = saveJob;
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
            bt_valider.IsEnabled = false;

            List<SaveJob> SaveJobs = _repository.Load();

            if (_saveJob != null)
            {
                SaveJobs.Remove(_saveJob);
            }

            var job = new SaveJob
            {
                SourcePath = SourcePathTextBox.Text.Trim(),
                DiskSerial = serial.Trim(),
                DestinationFolderName = DestinationFolderNameTextBox.Text.Trim()
            };

            SaveJobs.Add(job);
            _repository.Save(SaveJobs);
            _mainWindow.NavigateToJobsListPage();
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
            _mainWindow.NavigateToJobsListPage();
        }

        private void BrowseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() != true)
                return;
            SourcePathTextBox.Text = dialog.SelectedPath;
        }

        public async void loadCombo()
        {
            var drives = await DriveHelper.GetDrives();

            DiskComboBox.ItemsSource = drives;

            if (_saveJob != null)
            {
                // Sélectionner le disque correspondant au serial
                var serial = _saveJob.DiskSerial;
                var matchingDrive = drives.FirstOrDefault(d => DriveHelper.GetVolumeSerial(d.Substring(0, 2)) == serial);
                if (matchingDrive != null)
                {
                    DiskComboBox.SelectedItem = matchingDrive;
                }
            }
        }

        private void bt_actualiser_Click(object sender, RoutedEventArgs e)
        {
            loadCombo();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadCombo();

            if (_saveJob != null)
            {
                SourcePathTextBox.Text = _saveJob.SourcePath;
                DestinationFolderNameTextBox.Text = _saveJob.DestinationFolderName;
            }
        }
    }
}
