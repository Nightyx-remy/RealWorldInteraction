using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Reactive;
using ReactiveUI;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.NETCoreMVVMApp1.Models;
using ScottPlot.Avalonia;

namespace Avalonia.NETCoreMVVMApp1.ViewModels {
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {
        
        public MainWindowViewModel() {
            _year = DateTime.Today.Year;
            PrevYear = ReactiveCommand.Create(PrevYearFn);
            NextYear = ReactiveCommand.Create(NextYearFn);
            OpenWikipedia = ReactiveCommand.Create(OpenWikipediaFn);

            Drivers = new ObservableCollection<DriverModel>();

            PieChart = new AvaPlot();
            LoadAll();
        }
        
        private static HttpClient client = new();
        public new event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<DriverModel> Drivers { get; }
        
        private int _year;
        public int Year {
            get => _year;
            set {
                _year = value;
                OnPropertyChanged("Year");
                Drivers.Clear();
                LoadAll();
            }
        }

        public ReactiveCommand<Unit, Unit> PrevYear { get; }
        public ReactiveCommand<Unit, Unit> NextYear { get; }
        public ReactiveCommand<Unit, Unit> OpenWikipedia { get; }
        
        public AvaPlot PieChart { get; }

        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        void PrevYearFn() {
            if (Year <= 1950) return; // Min year 1950
            Year--;
        }

        void NextYearFn() {
            if (Year >= DateTime.Today.Year) return;
            Year++;
        }

        void OpenWikipediaFn() {
            Console.Out.WriteLine("Hello");
        }

        private async void LoadAll() {
            await LoadDrivers(Year);
            LoadPlot();
        }
        
        private async Task LoadDrivers(int year) {
            var response = await client.GetAsync($"https://ergast.com/api/f1/{year}/driverStandings");
            if (!response.IsSuccessStatusCode) return;

            var str = response.Content.ReadAsStringAsync().Result;
            Console.Out.WriteLine($"{str}");
                
            var doc = new XmlDocument();
            doc.LoadXml(str);

            var standings = doc.DocumentElement?["StandingsTable"]?["StandingsList"];

            if (standings == null) return;
            foreach (XmlElement driverStanding in standings?.ChildNodes) {
                var driver = driverStanding["Driver"];
                if (driver == null) continue;
                
                var constructor = driverStanding["Constructor"];
                if (constructor == null) continue;
                
                var firstName = driver["GivenName"]?.InnerText;
                var lastName = driver["FamilyName"]?.InnerText;
                var nationality = driver["Nationality"]?.InnerText;
                var team = constructor["Name"]?.InnerText;
                var url = driver.Attributes["url"]?.Value;
                int permanentNumber;
                double points;
                try {
                    points = Convert.ToDouble(driverStanding.Attributes["points"]?.Value);   
                    permanentNumber = Convert.ToInt32(driver["PermanentNumber"]?.InnerText);
                } catch (Exception e) {
                    Console.Out.WriteLine($"{driverStanding.Attributes["points"]?.Value}");
                    Console.Out.WriteLine($"{driver["PermanentNumber"]?.InnerText}");
                    points = 0;
                    permanentNumber = 0;
                }
                if (firstName != null && lastName != null && nationality != null && team != null && url != null)
                    Drivers.Add(new DriverModel() {
                        FirstName = firstName,
                        LastName = lastName,
                        Nationality = nationality,
                        Team = team,
                        Points = points,
                        PermanentNumber = permanentNumber,
                        URL = url
                    });
            }
        }

        public void LoadPlot() {
            var size = Drivers.Count;
            var points = new double [size];
            var driverNames = new string[size];
            var index = 0;
            var sum = 0.0;
            //getting all drivers points
            foreach (var driver in Drivers) {
                points[index] = driver.Points;
                sum = sum + points[index];
                driverNames[index] = driver.FullName;
                index++;
            }

            var sumOfOthers = 0.0;
            var lowPointCount = 0;
            //categorizing low driver points (below 2% of overall point sum) as "other"
             for (var i = 0; i < size; i++)
             {
                 if (0.02 > (points[i] / sum)) //condition for low driver points
                 {
                     sumOfOthers = sumOfOthers + points[i];
                     lowPointCount++;
                 }
             }
             var pointsToPlot = new double [size-lowPointCount];
             var driverNamesToPlot = new string[size-lowPointCount];
             for (var i = 0; i < pointsToPlot.Length-1; i++)
             {
                 if (0.02 < (points[i] / sum)) //condition for low driver points
                 {
                     pointsToPlot[i] = points[i];
                     driverNamesToPlot[i] = driverNames[i];
                 } 

            }
             pointsToPlot[^1] = sumOfOthers;
           driverNamesToPlot[^1] = "Others";
            PieChart.Plot.Style(ScottPlot.Style.Gray1);
            PieChart.Plot.Clear();
            PieChart.Plot.Title("Season point distribution");
            PieChart.Configuration.RightClickDragZoom = false;
            PieChart.Configuration.MiddleClickDragZoom = false;
            PieChart.Configuration.ScrollWheelZoom = false;
            PieChart.Configuration.LeftClickDragPan = false;
            var pie = PieChart.Plot.AddPie(pointsToPlot);
            
            pie.SliceLabels = driverNamesToPlot;
            pie.ShowLabels = true;
            pie.Explode = true;
            pie.DonutSize = 0.4;
        }
    }
}