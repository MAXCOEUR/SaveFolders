using Ookii.Dialogs.Wpf;
using SaveFolders.Helpers;
using SaveFolders.Models;
using SaveFolders.Repositories;
using SaveFolders.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour JobsListPage.xaml
    /// </summary>
    public partial class JobsListPage : UserControl
    {
        private readonly MainWindow _mainWindow;
        private ObservableCollection<SaveJob> SaveJobs { get; } = new();
        private SaveJobRepository _repository = new();

        public JobsListPage(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateToAddJobPage(null);
        }

        private async void Lancer_Click(object sender, RoutedEventArgs e)
        {
            foreach (JobItemPage job in SaveJobsPanel.Children)
            {
                await job.RunCopy(); // attend que la précédente se termine
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            loadListe();
        }
        private void loadListe()
        {
            SaveJobs.Clear();
            SaveJobsPanel.Children.Clear();
            foreach (var job in _repository.Load())
            {
                SaveJobs.Add(job);
                var jobControl = new JobItemPage(job);
                jobControl.EditRequested += JobItem_EditRequested;
                jobControl.DeleteRequested += JobItem_DeleteRequested;
                jobControl.SyncRequested += JobItem_SyncRequested;
                SaveJobsPanel.Children.Add(jobControl);
            }
        }

        private void JobItem_SyncRequested(object? sender, bool e)
        {
            bt_start.IsEnabled = !e;
            bt_ajouter.IsEnabled = !e;

            foreach (var child in SaveJobsPanel.Children)
            {
                if (child is JobItemPage jobItem)
                {
                    jobItem.isEnable(!e);
                }
            }

        }

        private void JobItem_EditRequested(object? sender, SaveJob job)
        {
            _mainWindow.NavigateToAddJobPage(job);
        }

        private void JobItem_DeleteRequested(object? sender, SaveJob job)
        {
            var result = MessageBox.Show(
                $"Voulez-vous vraiment supprimer :\n{job.SourcePath} ?",
                "Confirmation de suppression",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes && sender is JobItemPage control)
            {
                SaveJobs.Remove(job);
                _repository.Save(SaveJobs.ToList());
                loadListe();
            }
        }
    }
}
