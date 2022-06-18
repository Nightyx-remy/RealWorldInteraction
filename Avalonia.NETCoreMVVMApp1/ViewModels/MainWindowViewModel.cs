using System;
using System.Reactive;
using ReactiveUI;

namespace Avalonia.NETCoreMVVMApp1.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            DoTheThing = ReactiveCommand.Create(RunTheThing);
        }
        
        public string Greeting => "Welcome to Avalonia!";
    
        public ReactiveCommand<Unit, Unit> DoTheThing { get; }

        void RunTheThing() {
            Console.Out.WriteLine("Hello");
        }
    }
}