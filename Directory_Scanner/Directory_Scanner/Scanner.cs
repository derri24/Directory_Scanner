using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Directory_Scanner.Entities;

namespace Directory_Scanner;

public class Scanner
{
    private static void SetPercents(FileData fileData)
    {
        foreach (var child in fileData.Children)
        {
             SetPercents(child);
             if (fileData.Size > 0)
                 child.Percent = Math.Round(child.Size * 100 / fileData.Size, 1);
        }
    }

    private static void SetFolderSize(FileData fileData)
    {
        foreach (var child in fileData.Children)
            SetFolderSize(child);
        if (fileData.Type == Type.Directory)
            fileData.Size = fileData.Children.Sum(x => x.Size);
    }

    private static void DirectoryProcessing(object data)
    {
        var directoryInfo = new DirectoryInfo(((List<object>) data)[1].ToString());
        FileData fileData = new FileData(Type.Directory, directoryInfo.Name);
        fileData.Children = AnalyzingDirectories(((List<object>) data)[1].ToString());
        //fileData.Size = fileData.Children.Sum(x => x.Size);
        //Thread.Sleep(300);
        var fileTree = ((List<object>) data)[0];
        ((ConcurrentBag<FileData>) fileTree).Add(fileData);
    }

    private static ConcurrentBag<FileData> AnalyzingDirectories(string path)
    {
        ConcurrentBag<FileData> fileTree = new ConcurrentBag<FileData>();
        var filePaths = Directory.GetFiles(path);
        foreach (var filePath in filePaths)
        {
            var fileInfo = new FileInfo(filePath);
            FileData fileData = new FileData(Type.File, fileInfo.Name, fileInfo.Length);
            fileTree.Add(fileData);
        }

        var directoryPaths = Directory.GetDirectories(path);
        foreach (var directoryPath in directoryPaths)
        {
            List<object> data = new List<object>() {fileTree, directoryPath};
            Thread thread = new Thread(DirectoryProcessing);
            thread.Start(data);

            // var directoryInfo = new DirectoryInfo(directoryPath);
            // FileData fileData = new FileData(Type.Directory, directoryInfo.Name);
            // fileData.Children = AnalyzingDirectories(directoryPath);
            // fileData.Size = fileData.Children.Sum(x => x.Size);
            // fileTree.Add(fileData);
        }

        return fileTree;
    }


    public static ConcurrentBag<FileData> GetFileTree(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        FileData rootFileTree = new FileData(Type.Directory, directoryInfo.Name);
        rootFileTree.Children = AnalyzingDirectories(path);
        //rootFileTree.Size = rootFileTree.Children.Sum(x => x.Size);
        rootFileTree.Percent = 100;
        Thread.Sleep(2000);
        ConcurrentBag<FileData> fileTree = new ConcurrentBag<FileData>();
        fileTree.Add(rootFileTree);
        SetFolderSize(rootFileTree);
        //Thread.Sleep(300);
        SetPercents(rootFileTree);
        return fileTree;
    }
}