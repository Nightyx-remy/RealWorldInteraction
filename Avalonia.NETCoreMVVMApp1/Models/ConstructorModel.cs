using Avalonia.Controls;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class ConstructorModel
{
    public string Name { get; set; }
    
    public string Nationality { get; set; }

    public Image NationalFlag { get; set; }
    
    //TODO: implement a function, that goes through the selected year's events and calculates all the points
    //TODO: consider putting point calculation function to the SeasonModel
    public ushort Points { get; set; }
    
    //Each constructor has two drivers
    //TODO: decide how to include them. In a list? Seperate values?
    //public List<DriverModel>
    

}