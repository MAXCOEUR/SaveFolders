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
        public MainWindow()
        {
            InitializeComponent();
            jobsListPage = new JobsListPage(this);
            NavigateToJobsListPage();
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
    }
}