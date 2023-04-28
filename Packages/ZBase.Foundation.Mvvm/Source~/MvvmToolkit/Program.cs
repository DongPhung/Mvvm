﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MvvmToolkit
{
    public static class Program
    {
        public static void Main()
        {

        }
    }

    [ObservableObject]
    public partial class MyViewModel
    {
        [ObservableProperty]
        private int _age;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyCanExecuteChangedFor(nameof(GreetUserCommand))]
        private string? _firstName;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FullName))]
        [NotifyCanExecuteChangedFor(nameof(GreetUserCommand))]
        private string? _lastName;

        public string? FullName => $"{FirstName} {LastName}";

        [RelayCommand]
        private static void GreetUser(MyViewModel x)
        {
            Console.WriteLine($"Hello {x.FullName}");
        }

        private static bool Validate() => false;

        [RelayCommand(CanExecute = nameof(Validate))]
        private void DoX()
        {
        }
    }
}