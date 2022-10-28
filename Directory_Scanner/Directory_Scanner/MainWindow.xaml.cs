using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Directory_Scanner.Entities;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;

namespace Directory_Scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    //const int MaxThreadCount = 3;
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static FileDataModel ConvertToModel(
            FileData fileData)
        {
            string emoje;
            if (fileData.Type == Type.Directory)
                emoje = "📁";
            else
                emoje = "📄";
            FileDataModel fileDataModel =
                new FileDataModel(fileData.Type, fileData.Name, fileData.Percent, fileData.Size, emoje);

            foreach (var child in fileData.Children)
            {
                fileDataModel.Children.Add(ConvertToModel(child));
            }

            return fileDataModel;
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                StartBtn.IsEnabled = false;
                StartBtn.Foreground = Brushes.DimGray;
                
                CancelBtn.IsEnabled = true;
                CancelBtn.Foreground=Brushes.White;
                
                string path = dialog.SelectedPath;
                var rootFileTree = ConvertToModel(await Scanner.GetFileTree(path));
                ObservableCollection<FileDataModel> fileTree = new ObservableCollection<FileDataModel>();
                fileTree.Add(rootFileTree);
                treeView.ItemsSource = fileTree.ToList();
            }
            StartBtn.IsEnabled=true;
            StartBtn.Foreground=Brushes.White;
            
            CancelBtn.IsEnabled = false;
            CancelBtn.Foreground = Brushes.DimGray;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Scanner.CancelScan();
            CancelBtn.IsEnabled = false;
            CancelBtn.Foreground = Brushes.DimGray;
        }
    }
}