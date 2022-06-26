using Avalonia.Controls;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class ConstructorModel : IEntityModel {
    public string Name { get; }
    public string Nationality { get; }
    public double Points { get; set; }
    public string Url { get; }

    public ConstructorModel(string name, string nationality, double points, string url) {
        Name = name;
        Nationality = nationality;
        Points = points;
        Url = url;
    }

}