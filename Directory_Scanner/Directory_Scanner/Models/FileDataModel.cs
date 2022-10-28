using System.Collections.ObjectModel;

namespace Directory_Scanner;

public class FileDataModel : TreeViewItemBase
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