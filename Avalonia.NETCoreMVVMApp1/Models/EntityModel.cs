namespace Avalonia.NETCoreMVVMApp1.Models; 

public interface IEntityModel {
    
    public string Name { get; }
    public string Nationality { get; }
    public double Points { get; set; }
    public string Url { get; }
    
}