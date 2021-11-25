using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Runtime.CompilerServices;

namespace SerialPortApplication.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public Window parentView;
        public ViewModelBase(Window parentView)
        {
            this.parentView = parentView;
        }

        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
        }

        protected void SetProperty<T>(ref T storage, T value, Action onChange, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
            onChange?.Invoke();
        }

        protected void SetProperty<T>(ref T storage, T value, Action<T> onChange, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
            onChange?.Invoke(value);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => this.RaisePropertyChanged(propertyName);

    }
}
