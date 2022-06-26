using System;
using Bitmap = System.Drawing.Bitmap;
namespace Avalonia.NETCoreMVVMApp1.Models;

public class DriverModel : IEntityModel {
    public string FirstName { get; }
    public string LastName { get; }
    public string Name => $"{FirstName} {LastName}";
    public double Points { get; set; }
    public int PermanentNumber { get; set; }
    public string Url { get; }
    public Bitmap FlagImage { get; set; }
    public string Team { get; set; }
    public string Nationality { get; }

    public DriverModel(string firstName, string lastName, string url, string team, string nationality, int permanentNumber, double points, Bitmap flagImage) {
        FirstName = firstName;
        LastName = lastName;
        Url = url;
        Team = team;
        Nationality = nationality;
        PermanentNumber = permanentNumber;
        Points = points;
        FlagImage = flagImage;
    }
}