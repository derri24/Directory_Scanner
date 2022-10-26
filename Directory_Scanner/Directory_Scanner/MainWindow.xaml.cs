using System;
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
using Ookii.Dialogs.Wpf;

namespace Directory_Scanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    //const int MaxThreadCount = 3;
    public enum FileType
    {
        File,
        Directory
    }

    public class FileData : TreeViewItemBase
    {
        public FileType Type { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public string Emoji { get; set; }
        public double Percent { get; set; }
        public ObservableCollection<FileData> Children { get; set; }

        public FileData(FileType type, string name, string emoji)
        {
            Type = type;
            Name = name;
            Emoji = emoji;
        }

        public FileData(FileType type, string name, long size, string emoji) : this(type, name, emoji)
        {
            Size = size;
            Children = new ObservableCollection<FileData>();
        }
    }

    public class Scanner
    {
        
        public static void SetPercents(List<FileData> fileDatas)
        {
            foreach (var fileData in fileDatas)
            {
                foreach (var child in fileData.Children)
                    if (fileData.Size > 0)
                        child.Percent = Math.Round(child.Size * 100 / fileData.Size, 1);
                SetPercents(fileData.Children.ToList());
            }
        }

        public static  void DirectoryProcessing( string directoryPath,  ObservableCollection<FileData>  fileTree)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            FileData fileData = new FileData(FileType.Directory, directoryInfo.Name, "📁");
            fileData.Children = GetFileSystemDataRecursively(directoryPath);
            fileData.Size = fileData.Children.Sum(x => x.Size);
            fileTree.Add(fileData);
        }
       
        public static ObservableCollection<FileData> GetFileSystemDataRecursively(string path)
        {
            ObservableCollection<FileData> fileTree = new ObservableCollection<FileData>();
            var filePaths = Directory.GetFiles(path);
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                FileData fileData = new FileData(FileType.File, fileInfo.Name, fileInfo.Length, "📄");
                fileTree.Add(fileData);
            }

            var directoryPaths = Directory.GetDirectories(path);
            foreach (var directoryPath in directoryPaths)
            {
               // ThreadPool.QueueUserWorkItem(new WaitCallback(DirectoryProcessing(directoryPath, fileTree)));
                var directoryInfo = new DirectoryInfo(directoryPath);
                FileData fileData = new FileData(FileType.Directory, directoryInfo.Name, "📁");
                fileData.Children = GetFileSystemDataRecursively(directoryPath);
                fileData.Size = fileData.Children.Sum(x => x.Size);
                fileTree.Add(fileData);
            }

            return fileTree;
        }
    }


    public class TreeViewItemBase : INotifyPropertyChanged
    {
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        private bool isExpanded;

        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                if (value != isExpanded)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }


    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.SelectedPath;
                
                var directoryInfo = new DirectoryInfo(path);
                FileData rootFileTree = new FileData(FileType.Directory, directoryInfo.Name, "📁");
                rootFileTree.Children = Scanner.GetFileSystemDataRecursively(path);
                rootFileTree.Size = rootFileTree.Children.Sum(x => x.Size);
                rootFileTree.Percent = 100;
                List<FileData> fileTree = new List<FileData>();
                fileTree.Add(rootFileTree);
                Scanner.SetPercents(fileTree.ToList());
               treeView.ItemsSource = fileTree.ToList();
            }
        }
    }
}