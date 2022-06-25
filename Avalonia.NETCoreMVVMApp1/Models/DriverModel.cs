using System;
using Avalonia.Controls;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class DriverModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string FullName => FirstName + " " + LastName;

    //TODO: implement a function, that goes through the selected year's events and calculates all the points
    //TODO: consider putting point calculation function to the SeasonModel
    public double Points { get; set; }
    
    public byte Age { get; set; }
    
    public int PermanentNumber { get; set; }
    public string URL { get; set; }
    
    // TODO: Calculate age in the getter
    public DateTime DateOfBirth { get; set; }
    
    public string Team { get; set; }
    public string Nationality { get; set; }
    
    public Image NationalFlag { get; set; }
}