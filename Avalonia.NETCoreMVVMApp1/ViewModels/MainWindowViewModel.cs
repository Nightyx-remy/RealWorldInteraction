using System;
using System.Reactive;
using ReactiveUI;

namespace Avalonia.NETCoreMVVMApp1.ViewModels {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            MenuItem1 = ReactiveCommand.Create(MenuItem1Action);
            MenuItem2 = ReactiveCommand.Create(MenuItem2Action);
            MenuItem3 = ReactiveCommand.Create(MenuItem3Action);
        }
        
        public string Greeting => "Welcome to Avalonia!";
    
        public ReactiveCommand<Unit, Unit> MenuItem1 { get; }
        public ReactiveCommand<Unit, Unit> MenuItem2 { get; }
        public ReactiveCommand<Unit, Unit> MenuItem3 { get; }

        void MenuItem1Action() {
            Console.Out.WriteLine("Item 1");
        }

        void MenuItem2Action() {
            Console.Out.WriteLine("Item 2");
        }

        void MenuItem3Action() {
            Console.Out.WriteLine("Item 3");
        }
    }
}