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
        AddJobPage addJobPage;
        public MainWindow()
        {
            InitializeComponent();
            jobsListPage = new JobsListPage(this);
            addJobPage = new AddJobPage(this);
            NavigateTo(0);
        }

        public void NavigateTo(int numeroPage)
        {
            switch (numeroPage)
            {
                case 0:
                    MainFrame.Navigate(jobsListPage);
                    break;
                 case 1:
                    MainFrame.Navigate(addJobPage);
                    break;
            }
            
        }
    }
}