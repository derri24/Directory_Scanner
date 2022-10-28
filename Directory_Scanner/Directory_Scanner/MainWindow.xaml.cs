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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Directory_Scanner.Entities;
using Ookii.Dialogs.Wpf;

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

        public static ObservableCollection<FileDataModel> ConvertConcurrentBagToObservableCollection(
            ConcurrentBag<FileData> fileDatas)
        {
            ObservableCollection<FileDataModel> convertedFileData = new ObservableCollection<FileDataModel>();
            foreach (var fileData in fileDatas)
            {
                string emoje;
                if (fileData.Type == Type.Directory)
                    emoje = "📁";
                else
                    emoje = "📄";
                
                FileDataModel fileDataModel = new FileDataModel(fileData.Type, fileData.Name, fileData.Percent,fileData.Size, emoje);
                fileDataModel.Children= ConvertConcurrentBagToObservableCollection(fileData.Children);
                convertedFileData.Add(fileDataModel);
            }

            return convertedFileData;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.SelectedPath;
                var fileTree = ConvertConcurrentBagToObservableCollection(Scanner.GetFileTree(path));
                treeView.ItemsSource = fileTree.ToList();
            }
        }
    }
}