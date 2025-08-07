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
        private RobocopyService _robocopyService = new();

        public JobsListPage(MainWindow mainWindow)
        {
            InitializeComponent();

            _mainWindow = mainWindow;

            SaveJobsListBox.ItemsSource = SaveJobs;
        }

        private void Ajouter_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigateTo(1);
        }

        private void Supprimer_Click(object sender, RoutedEventArgs e)
        {
            var job = (SaveJob)SaveJobsListBox.SelectedItem;
            if (job == null)
            {
                MessageBox.Show("Sélectionnez un élément à supprimer.");
                return;
            }

            SaveJobs.Remove(job);
            _repository.Save(SaveJobs.ToList());
        }

        private void Lancer_Click(object sender, RoutedEventArgs e)
        {
            foreach (var job in SaveJobs)
                _robocopyService.RunCopy(job);

            MessageBox.Show("Sauvegarde terminée !");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SaveJobs.Clear();
            foreach (var job in _repository.Load())
                SaveJobs.Add(job);
        }
    }
}
