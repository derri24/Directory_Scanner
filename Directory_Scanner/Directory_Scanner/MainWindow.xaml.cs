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
using Directory_Scanner.Services;
using Ookii.Dialogs.Wpf;
using MessageBox = System.Windows.MessageBox;

namespace Directory_Scanner
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileTreeViewModel.SetButtons(this.StartBtn,this.CancelBtn);
            DataContext = new FileTreeViewModel(new DialogService(),new ConvertService());
        }
   
    }
}