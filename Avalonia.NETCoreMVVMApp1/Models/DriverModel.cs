using System;
using Avalonia.Controls;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class DriverModel
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    //TODO: implement a function, that goes through the selected year's events and calculates all the points
    //TODO: consider putting point calculation function to the SeasonModel
    public short Points { get; set; }
    
    public byte Age { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public string Nationality { get; set; }
    
    public Image NationalFlag { get; set; }
}