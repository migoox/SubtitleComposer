using System;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SubtitleComposer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Subtitle> Subtitles { get; set; } = new ObservableCollection<Subtitle>();
        public AppProperties AppProperties { get; set; } = new AppProperties();

        public bool VideoIsPlaying { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void FileOpenMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".mp4";
            dialog.Filter = "Video Files (*.mp4;*.mkv;*.avi;*.wmv;*.mov)|*.mp4;*.mkv;*.avi;*.wmv;*.mov|All Files (*.*)|*.*"; 
            
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                this.VideoPlayer.Source = new Uri(dialog.FileName);
                this.VideoPlayer.Play();
                VideoIsPlaying = true;
            }
        }

        private void FileCloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VideoPlayer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (VideoIsPlaying)
            {
                VideoPlayer.Pause();
                VideoIsPlaying = false;
            }
            else
            {
                this.VideoPlayer.Play();
                VideoIsPlaying = true;
            }
        }

        private void VideoPlayer_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Get the delta value of the mouse wheel rotation
            int delta = e.Delta;

            // Adjust the volume based on the delta value
            if (delta > 0)
            {
                // Increase the volume
                if (VideoPlayer.Volume < 1)
                    VideoPlayer.Volume += 0.1;
            }
            else if (delta < 0)
            {
                // Decrease the volume
                if (VideoPlayer.Volume > 0)
                    VideoPlayer.Volume -= 0.1;
            }
        }

        private void MainDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainDataGrid.SelectedItem == null)
                return;

            if (MainDataGrid.SelectedItem is not Subtitle)
                return;

            var selected = (Subtitle)MainDataGrid.SelectedItem;

            VideoPlayer.Pause();
            VideoIsPlaying = false;
            VideoPlayer.Position = selected.ShowTime;
        }
    }
}
