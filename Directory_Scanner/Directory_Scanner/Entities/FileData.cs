using System.Collections.Concurrent;

namespace Directory_Scanner.Entities;

public class FileData
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public double Size { get; set; }
    public double Percent { get; set; }
    public ConcurrentBag<FileData> Children { get; set; }

    public FileData(Type type, string name)
    {
        Type = type;
        Name = name;
    }
    
    public FileData(Type type, string name, double size) : this(type, name)
    {
        Size = size;
        Children = new ConcurrentBag<FileData>();
    }
}