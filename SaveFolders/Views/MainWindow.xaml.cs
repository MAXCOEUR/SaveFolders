using Ookii.Dialogs.Wpf;
using SaveFolders.Commands;
using SaveFolders.Helpers;
using SaveFolders.Models;
using SaveFolders.Repositories;
using SaveFolders.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        JobsListPage jobsListPage;
        LoaderPage loaderPage;
        UserControl? previousPage;

        private DeviceWatcher deviceWatcher;
        public MainWindow()
        {
            InitializeComponent();

            jobsListPage = new JobsListPage(this);
            loaderPage = new LoaderPage();

            DriveHelper.IsLoad += IsLoaderPage;
            DriveHelper.InitializeAsync();

            deviceWatcher = new DeviceWatcher(this);
            deviceWatcher.DeviceChanged += DeviceChanged;

        }

        private void DeviceChanged(object? sender, EventArgs e)
        {
            DriveHelper.InitializeAsync();
        }

        private void IsLoaderPage(object? sender, bool isLoading)
        {
            Dispatcher.Invoke(() =>
            {
                if (isLoading)
                {
                    // Sauvegarde de la page actuelle
                    previousPage = MainFrame.Content as UserControl;
                    MainFrame.Navigate(loaderPage);
                }
                else
                {
                    // Retour sur la page précédente si elle existe
                    if (previousPage != null)
                        MainFrame.Navigate(previousPage);
                    else
                        NavigateToJobsListPage();
                }
            });
        }

        public void NavigateToJobsListPage()
        {
            MainFrame.Navigate(jobsListPage);

        }
        public void NavigateToAddJobPage(SaveJob? saveJob)
        {

            AddJobPage addJobPage = new AddJobPage(this, saveJob);
            MainFrame.Navigate(addJobPage);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            deviceWatcher.Start();
        }
    }
}