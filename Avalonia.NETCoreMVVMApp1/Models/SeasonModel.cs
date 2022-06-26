using System.Collections.Generic;

namespace Avalonia.NETCoreMVVMApp1.Models;

public class SeasonModel
{
    public short Year { get; set; }
    
    public sbyte EventCount { get; set; }

    public List<EventModel> Events { get; set; } = new List<EventModel>();
}