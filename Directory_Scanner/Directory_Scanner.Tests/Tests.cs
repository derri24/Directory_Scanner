using System.Linq;
using Directory_Scanner.Library.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Directory_Scanner.Tests;

[TestClass]
public class Tests
{
    private FileData firstFileTree;
    private FileData secondFileTree;
    private FileData thirdFileTree;

    public Tests()
    {
        firstFileTree = Scanner.GetFileTree(@"C:\Users\derri\OneDrive\Рабочий стол\БГУИР").Result;
        secondFileTree = Scanner.GetFileTree(@"C:\Users\derri\OneDrive\Изображения").Result;
        thirdFileTree = Scanner.GetFileTree(@"C:\Users\derri\OneDrive\Рабочий стол\папка\папка 1").Result;
    }

    [TestMethod]
    public void CheckChildrenTest()
    {
        Assert.IsNotNull(firstFileTree.Children);
        Assert.AreEqual(firstFileTree.Children.Count, 3);
        Assert.IsNotNull(firstFileTree.Children.FirstOrDefault(child => child.Name == "1 курс"));
        Assert.AreEqual(firstFileTree.Children.FirstOrDefault(child => child.Name == "1 курс").Children.Count, 9);

        Assert.IsNotNull(secondFileTree.Children);
        Assert.AreEqual(secondFileTree.Children.Count, 19);

        Assert.IsNotNull(thirdFileTree.Children);
        Assert.AreEqual(thirdFileTree.Children.Count, 3);
    }

    [TestMethod]
    public void CheckTypesTest()
    {
        Assert.AreEqual(firstFileTree.Type, Type.Directory);
        var childFirstTree = firstFileTree.Children.FirstOrDefault(child => child.Name == "2 курс");
        Assert.AreEqual(childFirstTree.Type, Type.Directory);

        Assert.AreEqual(secondFileTree.Type, Type.Directory);

        var childThirdTree = thirdFileTree.Children.FirstOrDefault(child => child.Name == "папка 1.2");
        Assert.AreEqual(childThirdTree.Type, Type.Directory);

        var grandChildThirdTree = childThirdTree.Children.FirstOrDefault(child => child.Name == "файл 1.txt");
        Assert.AreEqual(grandChildThirdTree.Type, Type.File);
    }

    [TestMethod]
    public void CheckSizesTest()
    {
        Assert.AreEqual(firstFileTree.Size, 11392286453);

        var childFirstTree = firstFileTree.Children.FirstOrDefault(child => child.Name == "2 курс");
        Assert.AreEqual(childFirstTree.Size, 9491919452);
        Assert.AreEqual(childFirstTree.Children.First(child => child.Name == "3 сем").Size, 1310731962);

        Assert.AreEqual(secondFileTree.Size, 325581357);
        Assert.AreEqual(thirdFileTree.Size, 409);
        Assert.AreEqual(thirdFileTree.Children.FirstOrDefault(child => child.Name == "папка 1.2").Size, 365);
        Assert.AreEqual(thirdFileTree.Children.FirstOrDefault(child => child.Name == "файл 1.txt").Size, 42);
    }

    [TestMethod]
    public void CheckPercentsTest()
    {
        var childFirstTree = firstFileTree.Children.FirstOrDefault(child => child.Name == "2 курс");
        var grandchildFirstTree = childFirstTree.Children.First(child => child.Name == "Task");

        Assert.IsTrue(childFirstTree.Percent > 80 && childFirstTree.Percent < 85);
        Assert.AreEqual(firstFileTree.Children.Sum(child => child.Percent), 100);
        Assert.AreEqual(grandchildFirstTree.Children.Sum(child => child.Percent), 100);

        Assert.AreEqual(thirdFileTree.Children.Sum(child => child.Percent), 100);

        var childThirdTree = thirdFileTree.Children.FirstOrDefault(child => child.Name == "папка 1.2");
        Assert.AreEqual(childThirdTree.Children.Sum(child => child.Percent), 100);
    }
}