using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SubtitleComposer
{
    public class Subtitle : INotifyPropertyChanged
    {
        // Declare event
        public event PropertyChangedEventHandler? PropertyChanged;

        private TimeSpan _showTime = new TimeSpan(0, 0, 0);

        public TimeSpan ShowTime
        {
            get
            {
                return _showTime;
            }
            set
            {
                _showTime = value;

                // notify itself
                OnPropertyChanged();

                // notify Duration property
                OnPropertyChanged("Duration");
            }
        }

        private TimeSpan _hideTime;

        public TimeSpan HideTime
        {
            get
            {
                return _hideTime;
            }
            set
            {
                _hideTime = value;
                
                // notify itself
                OnPropertyChanged();

                // notify Duration property
                OnPropertyChanged("Duration");
            }
        }

        public TimeSpan Duration 
        {
            get
            {
                if (ShowTime > HideTime)
                    return new TimeSpan(0, 0, 0);

                return HideTime - ShowTime;
            }
            set
            {
                HideTime = ShowTime + value;
            }
        }

        private string _text = "";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        private string _translation = "";
        public string Translation
        {
            get
            {
                return _translation;
            }
            set
            {
                _translation = value;
                OnPropertyChanged();
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}