using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.NETCoreMVVMApp1.Models;
using ScottPlot.Avalonia;

namespace Avalonia.NETCoreMVVMApp1.ViewModels {
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {
        
        public MainWindowViewModel() {
            // Initialize the Year
            _year = DateTime.Today.Year;
            
            // Initialize all the commands
            PrevYear = ReactiveCommand.Create(PrevYearFn);
            NextYear = ReactiveCommand.Create(NextYearFn);
            OpenWikipedia = ReactiveCommand.Create<IEntityModel>(OpenWikipediaFn);

            // Initialize the Lists
            Drivers = new ObservableCollection<DriverModel>();
            Constructors = new ObservableCollection<ConstructorModel>();

            // Initialize the plots
            DriverChart = new AvaPlot();
            TeamChart = new AvaPlot();
            SetupPlot(DriverChart, "Driver's points distribution");
            SetupPlot(TeamChart, "Team's points distribution");
            
            // Load all the data
            LoadAll();
        }
        
        private static readonly HttpClient Client = new(); // Used to call the API
        
        // Used to update the UI when a property is updated
        public new event PropertyChangedEventHandler? PropertyChanged; 
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
        // List of Drivers and Constructors used in the Datagrids
        public ObservableCollection<DriverModel> Drivers { get; }
        public ObservableCollection<ConstructorModel> Constructors { get; }
        
        // Everything related to the selected Year
        public ReactiveCommand<Unit, Unit> PrevYear { get; }
        public ReactiveCommand<Unit, Unit> NextYear { get; }
        private int _year;
        public int Year {
            get => _year;
            set {
                _year = value;
                OnPropertyChanged("Year");
                Drivers.Clear();
                Constructors.Clear();
                LoadAll();
            }
        }

        private void PrevYearFn() {
            if (Year <= 1950) return; // Min year 1950
            Year--;
        }

        private void NextYearFn() {
            if (Year >= DateTime.Today.Year) return;
            Year++;
        }
        
        // Command to open the wikipedia page of a given Entity
        public ReactiveCommand<IEntityModel, Unit> OpenWikipedia { get; }

        private void OpenWikipediaFn(IEntityModel entity) {
            Process.Start(new ProcessStartInfo {
                FileName = entity.Url,
                UseShellExecute = true
            });
        }
        
        // Plots
        public AvaPlot DriverChart { get; }
        public AvaPlot TeamChart { get; }

        // Load all the drivers and teams to the lists and generate the plots
        private async void LoadAll() {
            await LoadDriversAndTeams(Year);
            LoadPlot(DriverChart, Drivers.Select(x => x as IEntityModel).ToList());
            LoadPlot(TeamChart, Constructors.Select(x => x as IEntityModel).ToList());
        }
        
        // Load all the drivers and teams to the lists
        private async Task LoadDriversAndTeams(int year) {
            var response = await Client.GetAsync($"https://ergast.com/api/f1/{year}/driverStandings");
            if (!response.IsSuccessStatusCode) return;

            var str = response.Content.ReadAsStringAsync().Result;
                
            var doc = new XmlDocument();
            doc.LoadXml(str);

            var standings = doc.DocumentElement?["StandingsTable"]?["StandingsList"];

            if (standings == null) return;
            foreach (XmlElement driverStanding in standings.ChildNodes) {
                var driver = driverStanding["Driver"];
                if (driver == null) continue;

                var constructor = driverStanding["Constructor"];
                if (constructor == null) continue;

                var firstName = driver["GivenName"]?.InnerText;
                var lastName = driver["FamilyName"]?.InnerText;
                var nationality = driver["Nationality"]?.InnerText;
                var team = constructor["Name"]?.InnerText;
                var teamNationality = constructor["Nationality"]?.InnerText;
                var url = driver.Attributes["url"]?.Value;
                var teamUrl = constructor.Attributes["url"]?.Value;
                int permanentNumber;
                double points;
                try {
                    points = Convert.ToDouble(driverStanding.Attributes["points"]?.Value);
                    permanentNumber = Convert.ToInt32(driver["PermanentNumber"]?.InnerText);
                } catch (Exception) {
                    points = 0;
                    permanentNumber = 0;
                }

                if (firstName == null || lastName == null || nationality == null || team == null || url == null ||
                    teamNationality == null || teamUrl == null) continue;
                Drivers.Add(new DriverModel(firstName, lastName, url, team, nationality, permanentNumber, points));
                foreach (var c in Constructors) {
                    if (c.Name != team) continue;
                    c.Points += points;
                    goto EOL;
                }
                Constructors.Add(new ConstructorModel(team, teamNationality, points, teamUrl));
                EOL: ;
            }
        }

        private void SetupPlot(AvaPlot plot, string name) {
            plot.Plot.Style(dataBackground: ColorTranslator.FromHtml("#383838"));
            plot.Plot.Title(name, true, Color.White);
            plot.Configuration.RightClickDragZoom = false;
            plot.Configuration.MiddleClickDragZoom = false;
            plot.Configuration.ScrollWheelZoom = false;
            plot.Configuration.LeftClickDragPan = false;
        }

        private void LoadPlot(AvaPlot plot, IList<IEntityModel> list) {
            var size = list.Count;
            var points = new double [size];
            var names = new string[size];
            var sum = 0.0;
            
            // Getting all the points and names
            var index = 0;
            foreach (var entity in list) {
                points[index] = entity.Points;
                sum += points[index];
                names[index] = entity.Name;
                index++;
            }

            //categorizing low driver points (below 2% of overall point sum) as "other"
            var sumOfOthers = 0.0;
            var lowPointCount = 0;
            for (var i = 0; i < size; i++) { 
                //condition for low driver points
                if (0.02 <= points[i] / sum) continue;
                sumOfOthers += points[i];
                lowPointCount++;
            }
             
            var pointsToPlot = new double [size-lowPointCount];
            var namesToPlot = new string[size-lowPointCount];
            for (var i = 0; i < pointsToPlot.Length-1; i++) {
                //condition for low driver points
                if (0.02 >= points[i] / sum) continue;
                pointsToPlot[i] = points[i];
                namesToPlot[i] = names[i];
            }

            // Apply data to the plot
            pointsToPlot[^1] = sumOfOthers;
            namesToPlot[^1] = "Others";
            
            plot.Plot.Clear();

            // Generate Pie chart
            var pie = plot.Plot.AddPie(pointsToPlot);
            pie.SliceLabels = names;
            pie.ShowLabels = true;
            pie.Explode = true;
            pie.DonutSize = 0.4;
            
            // Refresh the chart (to apply changes)
            plot.Refresh();
        }
    }
}