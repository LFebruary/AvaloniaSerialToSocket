// AvaloniaSerialToSocket https://github.com/LFebruary/AvaloniaSerialToSocket 
// (c) 2024 Lyle February 
// Released under the MIT License

using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using ReactiveUI;

namespace SerialToSocket.AvaloniaApp.ViewModels
{
    /// <summary>
    /// Base class for view models, providing common functionality for property change notification.
    /// </summary>
    /// <param name="parentView">The parent view associated with the view model.</param>
    public class ViewModelBase(Window parentView) : ReactiveObject
    {
        /// <summary>
        /// The parent view associated with the view model.
        /// </summary>
        public readonly Window ParentView = parentView;

        /// <summary>
        /// Sets the value of a property and raises the PropertyChanged event if the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="propertyName">The name of the property. Automatically determined if not specified.</param>
        protected void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            this.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Sets the value of a property, raises the PropertyChanged event, and invokes an action if the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="onChange">The action to invoke when the property value changes.</param>
        /// <param name="propertyName">The name of the property. Automatically determined if not specified.</param>
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

        /// <summary>
        /// Sets the value of a property, raises the PropertyChanged event, and invokes an action with the new value if the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="onChange">The action to invoke with the new value when the property value changes.</param>
        /// <param name="propertyName">The name of the property. Automatically determined if not specified.</param>
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

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to raise the event for. Automatically determined if not specified.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => this.RaisePropertyChanged(propertyName);

    }
}
