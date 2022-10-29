using Ookii.Dialogs.Wpf;

namespace Directory_Scanner.Services;

public interface IDialogService
{
    public string DirectoryPath { get; set; }
    public bool OpenDirectoryDialog();
}
public class DialogService:IDialogService
{
   public string DirectoryPath { get; set; }   
   
   public bool OpenDirectoryDialog()
   {
       var dialog = new VistaFolderBrowserDialog();
       if (dialog.ShowDialog() == true)
       {
           DirectoryPath = dialog.SelectedPath;
           return true;
       }
       return false;
   }
}