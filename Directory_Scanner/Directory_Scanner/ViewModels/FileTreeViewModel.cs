using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Directory_Scanner.Library.Entities;
using Directory_Scanner.Services;

namespace Directory_Scanner;

public class FileTreeViewModel : INotifyPropertyChanged
{
    public ObservableCollection<FileDataModel> FileDataModels { get; set; }

    private static Button StartBtn;
    private static Button CancelBtn;

    public static void SetButtons(Button startBtn, Button cancelBtn)
    {
        StartBtn = startBtn;
        CancelBtn = cancelBtn;
    }

    private IDialogService dialogService;
    private IConvertService convertService;

    public FileTreeViewModel(IDialogService dialogService, IConvertService convertService)
    {
        this.dialogService = dialogService;
        this.convertService = convertService;
        FileDataModels = new ObservableCollection<FileDataModel>();
    }

    private RelayCommand startCommand;

    public RelayCommand StartCommand
    {
        get
        {
            return startCommand =
                   (startCommand = new RelayCommand(async obj =>
                   {
                       if (dialogService.OpenDirectoryDialog() == true)
                       {
                           StartBtn.IsEnabled = false;
                           StartBtn.Foreground = Brushes.DimGray;

                           CancelBtn.IsEnabled = true;
                           CancelBtn.Foreground = Brushes.White;

                           var rootFileTree =
                               convertService.ConvertToModel(await Scanner.GetFileTree(dialogService.DirectoryPath));
                           FileDataModels.Clear();
                           FileDataModels.Add(rootFileTree);
                       }

                       StartBtn.IsEnabled = true;
                       StartBtn.Foreground = Brushes.White;

                       CancelBtn.IsEnabled = false;
                       CancelBtn.Foreground = Brushes.DimGray;
                   }));
        }
    }


    private RelayCommand cancelCommand;

    public RelayCommand CancelCommand
    {
        get
        {
            return cancelCommand =
                   (cancelCommand = new RelayCommand(async obj =>
                   {
                       Scanner.CancelScan();
                       CancelBtn.IsEnabled = false;
                       CancelBtn.Foreground = Brushes.DimGray;
                   }));
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
}