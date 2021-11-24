using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerialPortApplication.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
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

        protected void OnPropertyChanged(string propertyName) => this.RaisePropertyChanged(propertyName);

    }
}
