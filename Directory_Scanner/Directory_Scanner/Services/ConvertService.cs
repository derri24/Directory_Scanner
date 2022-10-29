
using Directory_Scanner.Library.Entities;

namespace Directory_Scanner.Services;

public interface IConvertService
{
    public FileDataModel ConvertToModel(FileData fileData);
}

public class ConvertService : IConvertService
{
    public FileDataModel ConvertToModel(FileData fileData)
    {
        string emoje;
        if (fileData.Type == Type.Directory)
            emoje = "📁";
        else
            emoje = "📄";
        FileDataModel fileDataModel =
            new FileDataModel(fileData.Type, fileData.Name, fileData.Percent, fileData.Size, emoje);

        foreach (var child in fileData.Children)
            fileDataModel.Children.Add(ConvertToModel(child));
        return fileDataModel;
    }
}