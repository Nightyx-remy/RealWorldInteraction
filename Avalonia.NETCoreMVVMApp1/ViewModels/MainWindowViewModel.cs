using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using ReactiveUI;
using System.Net.Http;
using System.Xml;
using Avalonia.NETCoreMVVMApp1.Models;

namespace Avalonia.NETCoreMVVMApp1.ViewModels {
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged {
        public MainWindowViewModel() {
            _year = DateTime.Today.Year;
            PrevYear = ReactiveCommand.Create(PrevYearFn);
            NextYear = ReactiveCommand.Create(NextYearFn);

            Drivers = new ObservableCollection<DriverModel>();
            
            LoadDrivers(Year);
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
                LoadDrivers(Year);
            }
        }

        public ReactiveCommand<Unit, Unit> PrevYear { get; }
        public ReactiveCommand<Unit, Unit> NextYear { get; }

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
        
        private async void LoadTeams(int year) {

            /*var drivers = doc.DocumentElement?["DriverTable"];
            foreach (XmlElement driver in drivers?.ChildNodes) {
                var firstName = driver["GivenName"]?.InnerText;
                var lastName = driver["FamilyName"]?.InnerText;
                var nationality = driver["Nationality"]?.InnerText;
                if (firstName != null && lastName != null && nationality != null)
                    Drivers.Add(new DriverModel() {
                        FirstName = firstName,
                        LastName = lastName,
                        Nationality = nationality
                    });
            }*/
        }
        
        private async void LoadDrivers(int year) {
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
                decimal points;
                try {
                    points = Convert.ToDecimal(driverStanding.Attributes["points"]?.Value);   
                } catch (Exception e) {
                    Console.Out.WriteLine($"{driverStanding.Attributes["points"]?.Value}");
                    points = 0;
                }
                if (firstName != null && lastName != null && nationality != null && team != null)
                    Drivers.Add(new DriverModel() {
                        FirstName = firstName,
                        LastName = lastName,
                        Nationality = nationality,
                        Team = team,
                        Points = points,
                    });
            }
            
            /*var response = await client.GetAsync($"https://ergast.com/api/f1/{year}/drivers");
            if (!response.IsSuccessStatusCode) return;
            
            var str = response.Content.ReadAsStringAsync().Result;
                
            var doc = new XmlDocument();
            doc.LoadXml(str);

            var drivers = doc.DocumentElement?["DriverTable"];
            foreach (XmlElement driver in drivers?.ChildNodes) {
                var firstName = driver["GivenName"]?.InnerText;
                var lastName = driver["FamilyName"]?.InnerText;
                var nationality = driver["Nationality"]?.InnerText;
                if (firstName != null && lastName != null && nationality != null)
                    Drivers.Add(new DriverModel() {
                        FirstName = firstName,
                        LastName = lastName,
                        Nationality = nationality
                    });
            }*/
        }
        
    }
}