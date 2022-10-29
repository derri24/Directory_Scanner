using System.Collections.ObjectModel;
using Directory_Scanner.Services;

namespace Directory_Scanner;

public class FileDataModel 
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public double Size { get; set; }
    public string Emoji { get; set; }
    public double Percent { get; set; }
    public ObservableCollection<FileDataModel> Children { get; set; }
    public FileDataModel(Type type, string name, double percent,double size, string emoji)
    {
        Type = type;
        Name = name;
        Percent = percent;
        Emoji = emoji;
        Size = size;
        Children = new ObservableCollection<FileDataModel>();
    }
}