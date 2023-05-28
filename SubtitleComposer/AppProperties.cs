using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleComposer
{
    public class AppProperties : INotifyPropertyChanged
    {
        // Declare event
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _showTranslation = false;

        public bool ShowTranslation
        {
            get
            {
                return _showTranslation;
            }
            set
            {
                _showTranslation = value;
                OnPropertyChanged();
            }
        }

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
