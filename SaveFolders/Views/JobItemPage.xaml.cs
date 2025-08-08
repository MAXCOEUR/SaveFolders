using SaveFolders.Models;
using SaveFolders.Services;
using System;
using System.Collections.Generic;
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
using static System.Windows.Forms.AxHost;

namespace SaveFolders.Views
{
    /// <summary>
    /// Logique d'interaction pour JobItemPage.xaml
    /// </summary>
    public partial class JobItemPage : UserControl
    {
        private readonly SaveJob _saveJob;
        private RobocopyService _robocopyService = new();

        // Événements publics
        public event EventHandler<SaveJob>? EditRequested;
        public event EventHandler<SaveJob>? DeleteRequested;

        public JobItemPage(SaveJob saveJob)
        {
            InitializeComponent();
            _saveJob = saveJob;
            _robocopyService.FinishRequested += FinishTraitement;
            _robocopyService.ProgressChanged += ProgressChanged;
        }

        private void ProgressChanged(int index)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Visibility = Visibility.Visible;
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Value = index;
            });
        }

        private void FinishTraitement(object? sender, bool e)
        {
            if(e)
                UpdateSyncStatus(2);
            else
                UpdateSyncStatus(0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SourcePathText.Text = _saveJob.SourcePath;
            DestinationPathText.Text = _saveJob.ResolveDestinationPath();
            UpdateSyncStatus(0);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, _saveJob);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditRequested?.Invoke(this, _saveJob);
        }

        public void runCopy()
        {
            UpdateSyncStatus(1);
            _robocopyService.RunCopy(_saveJob);
            
        }

        public void UpdateSyncStatus(int state)
        {
            Dispatcher.Invoke(() =>
            {
                if (state == 0)
                {
                    SyncStatusIcon.Text = "❌";
                    SyncStatusIcon.Foreground = Brushes.Red;
                    SyncStatusIcon.ToolTip = "Non synchronisé";
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
                else if (state == 1)
                {
                    SyncStatusIcon.Text = "🔄";
                    SyncStatusIcon.Foreground = Brushes.Blue;
                    SyncStatusIcon.ToolTip = "Synchronisation en cours...";
                    ProgressBar.Visibility = Visibility.Visible;
                    ProgressBar.IsIndeterminate = true;
                }
                else if (state == 2)
                {
                    SyncStatusIcon.Text = "✅";
                    SyncStatusIcon.Foreground = Brushes.Green;
                    SyncStatusIcon.ToolTip = "Synchronisé avec succès";
                    ProgressBar.Visibility = Visibility.Collapsed;
                }
            });
        }


        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            runCopy();
        }
    }

}
