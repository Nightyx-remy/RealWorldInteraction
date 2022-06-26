using System;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class EventModel
{
    public string CircuitName { get; set; }
    
    // or country
    public string Location { get; set; }
    
    public DateTime EventDate { get; set; }
    
    public byte RoundInTheSeason { get; set; }
    
    // TODO: from the API, determine if it is possible to include Weather conditions
    // public string WeatherCondition {get; set;}
    
    public byte Laps { get; set; }
    
    //TODO: Qualifying Results?
    
    //TODO: Race Results?
    
    
}