﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        var fileTree = ((List<object>) data)[0];
        ((ConcurrentBag<FileData>) fileTree).Add(fileData);
        Interlocked.Decrement(ref runningCount);
    }

    
    private static int runningCount;

    private static string[] CheckAccess(string path)
    {
        string[] filePaths;
        try
        {
            filePaths = Directory.GetFiles(path);
        }
        catch
        {
            return null;
        }
        return filePaths;
    }
    private static ConcurrentBag<FileData> AnalyzingDirectories(string path)
    {
        ConcurrentBag<FileData> fileTree = new ConcurrentBag<FileData>();
        string[] filePaths = CheckAccess(path);
        if (filePaths.Length==0)
            return fileTree;

        foreach (var filePath in filePaths)
        {
            var fileInfo = new FileInfo(filePath);
            FileData fileData = new FileData(Type.File, fileInfo.Name, fileInfo.Length);
            fileTree.Add(fileData);
        }

        string[] directoryPaths = Directory.GetDirectories(path);
        foreach (var directoryPath in directoryPaths)
        {
            if (token.IsCancellationRequested)
                return fileTree;
            
            List<object> data = new List<object>() {fileTree, directoryPath};
            Interlocked.Increment(ref runningCount);
            Monitor.Enter(data);
            ThreadPool.QueueUserWorkItem(DirectoryProcessing, data);
        }

        return fileTree;
    }

    private static CancellationTokenSource cancelTokenSource;
    private static CancellationToken token;
    public static async Task<FileData> GetFileTree(string path)
    {
        FileData rootFileTree = null;
        await Task.Run(() =>
        {
            var directoryInfo = new DirectoryInfo(path);
            rootFileTree = new FileData(Type.Directory, directoryInfo.Name);
            rootFileTree.Children = AnalyzingDirectories(path);

            rootFileTree.Percent = 100;
            while (runningCount > 0) { }

            if (token.IsCancellationRequested)
            {
                cancelTokenSource.Dispose();
                token=new CancellationToken();
            }

            SetFolderSize(rootFileTree);
            SetPercents(rootFileTree);
        });
        return rootFileTree;
    }


    public static void CancelScan()
    {
        cancelTokenSource = new CancellationTokenSource();
        token = cancelTokenSource.Token;
        cancelTokenSource.Cancel();
    }
}
//отмена количество потоков