using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SubtitleComposer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Subtitle> Subtitles { get; set; } = new ObservableCollection<Subtitle>();
        public AppProperties AppProperties { get; set; } = new AppProperties();

        // timer that runs on the UI thread and raises events at specified intervals
        // used in order to update slider and textblock
        private DispatcherTimer _videoTimer = new DispatcherTimer();

        // used in order to update displayed subtitles
        private DispatcherTimer _subtitlesTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            UpdateVideoElapsedTimeTextBlock();
        }

        private void FileOpenMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document";
            dialog.DefaultExt = ".mp4";
            dialog.Filter =
                "Video Files (*.mp4;*.mkv;*.avi;*.wmv;*.mov)|*.mp4;*.mkv;*.avi;*.wmv;*.mov|All Files (*.*)|*.*";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                VideoPlayer.Source = new Uri(dialog.FileName);
                VideoPlayer.Play();
                AppProperties.VideoIsPlaying = true;
            }
        }

        private void FileCloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void VideoPlayer_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AppProperties.VideoIsPlaying)
            {
                this.PauseVideo();
            }
            else
            {
                this.PlayVideo();
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

            VideoPlayerSlider.ValueChanged -= VideoPlayerVolumeSlider_OnValueChanged;
            VideoPlayerVolumeSlider.Value = 1.0 / VideoPlayer.Volume;
            VideoPlayerSlider.ValueChanged += VideoPlayerVolumeSlider_OnValueChanged;
        }

        private void PlayVideo()
        {
            VideoPlayer.Play();
            AppProperties.VideoIsPlaying = true;
            _videoTimer.Start();
        }

        private void StopVideo()
        {
            VideoPlayer.Stop();
            AppProperties.VideoIsPlaying = false;
            _videoTimer.Stop();
            UpdateVideoElapsedTimeTextBlock();
        }

        private void PauseVideo()
        {
            VideoPlayer.Pause();
            AppProperties.VideoIsPlaying = false;
            _videoTimer.Stop();
            UpdateVideoElapsedTimeTextBlock();
        }

        private void UpdateVideoElapsedTimeTextBlock()
        {
            ElapsedTimeTextBlock.Text = VideoPlayer.Position.ToString("h\\:mm\\:ss\\.fff");
        }

        private void MainDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainDataGrid.SelectedItem == null)
                return;

            if (MainDataGrid.SelectedItem is not Subtitle)
                return;

            var selected = (Subtitle)MainDataGrid.SelectedItem;

            VideoPlayer.Pause();
            AppProperties.VideoIsPlaying = false;
            VideoPlayer.Position = selected.ShowTime;
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Author: Michał Okurowski 2023", "Subtitle Composer - About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MainDataGrid_OnInitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            Subtitle item = e.NewItem as Subtitle;
            if (item == null)
                return;

            var maxTimeSpan = Subtitles.Select(st => st.HideTime).Max();
            item.HideTime = maxTimeSpan;
            item.ShowTime = maxTimeSpan;
            item.Text = "";
            item.Translation = "";
        }

        private void VideoPlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.PlayVideo();
        }

        private void VideoStopButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.StopVideo();
        }

        private void VideoPauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.PauseVideo();
        }

        private void VideoPlayer_OnMediaOpened(object sender, RoutedEventArgs e)
        {
            AppProperties.VideoTotalTime = VideoPlayer.NaturalDuration.TimeSpan;

            _videoTimer = new DispatcherTimer();
            _videoTimer.Interval = TimeSpan.FromMilliseconds(10);
            _videoTimer.Tick += VideoTimer_Tick;

            VideoPlayerVolumeSlider.Value = 1.0 / VideoPlayer.Volume;

            _subtitlesTimer = new DispatcherTimer();
            _subtitlesTimer.Interval = TimeSpan.FromMilliseconds(100);
            _subtitlesTimer.Tick += SubtitlesTimer_Tick;

            _subtitlesTimer.Start();

            this.PlayVideo();
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            // temporarily unsubscribe the event (on value changed event will be produced only on user slider value change)
            VideoPlayerSlider.ValueChanged -= VideoPlayerSlider_OnValueChanged;

            VideoPlayerSlider.Value = VideoPlayer.Position.TotalMilliseconds / AppProperties.VideoTotalTime.TotalMilliseconds;

            UpdateVideoElapsedTimeTextBlock();

            VideoPlayerSlider.ValueChanged += VideoPlayerSlider_OnValueChanged;
        }

        private void SubtitlesTimer_Tick(object sender, EventArgs e)
        {
            StringBuilder subtitlesBuilder = new StringBuilder();

            foreach (var subtitle in Subtitles
                         .Select(st => st)
                         .Where(st=> st.ShowTime <= VideoPlayer.Position && st.HideTime >= VideoPlayer.Position && st.Text != "")
                         .OrderBy(st => st.ShowTime))
            {
                subtitlesBuilder.Append(subtitle.Text).Append("\n");
            }

            if (subtitlesBuilder.Length != 0)
            {
                subtitlesBuilder.Remove(subtitlesBuilder.Length - 1, 1);
                this.VideoSubtitlesTextBlock.Text = subtitlesBuilder.ToString();
            }
            else
            {
                this.VideoSubtitlesTextBlock.Text = "";
            }

            if (this.VideoSubtitlesTextBlock.Text == "")
            {
                VideoSubtitlesTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                VideoSubtitlesTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void VideoPlayerSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int totalMilliseconds = (int)(VideoPlayerSlider.Value * AppProperties.VideoTotalTime.TotalMilliseconds);
            VideoPlayer.Position = TimeSpan.FromMilliseconds(totalMilliseconds);
            UpdateVideoElapsedTimeTextBlock();
        }

        private void VideoPlayerVolumeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoPlayer.Volume = VideoPlayerVolumeSlider.Value;
        }

        private void AddDataGridContextMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            TimeSpan maxTimeSpan;
            if (Subtitles.Count == 0)
            {
                maxTimeSpan = new TimeSpan(0, 0, 0);
            }
            else
            {
                maxTimeSpan = Subtitles.Select(st => st.HideTime).Max();
            }

            Subtitles.Add(new Subtitle(maxTimeSpan, maxTimeSpan, "", ""));
        }

        private void AddAfterDataGridContextMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            TimeSpan maxTimeSpan = new TimeSpan(0,0,0);

            foreach (var elem in MainDataGrid.SelectedItems)
            {
                var subtitle = (Subtitle)elem;
                if (subtitle.HideTime > maxTimeSpan)
                {
                    maxTimeSpan = subtitle.HideTime;
                }
            }

            Subtitles.Add(new Subtitle(maxTimeSpan, maxTimeSpan, "", ""));
        }

        private void DeleteDataGridContextMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            while (MainDataGrid.SelectedItems.Count > 0)
            {
                var subtitle = (Subtitle)MainDataGrid.SelectedItems[MainDataGrid.SelectedItems.Count - 1];
                Subtitles.Remove(subtitle);
            }
        }
    }
}
