using System.ComponentModel;

namespace Directory_Scanner;

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