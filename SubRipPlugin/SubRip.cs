using SubtitleComposer;
using System;
using System.Collections.Generic;
using System.IO;

namespace SubRipPlugin
{
    public class SubRipPlugin : ISubtitlesPlugin
    {
        public string Name { get => "SubRip"; }
        public string Extension { get => ".srt"; }

        public ICollection<Subtitle> Load(string path)
        {
            List<Subtitle> subtitles = new List<Subtitle>();

            try
            {
                string[] lines = File.ReadAllLines(path);

                for (int i = 0; i < lines.Length; i += 3)
                {
                    string timeString = lines[i + 1].Replace(',', '.');
                    string[] times = timeString.Split(" --> ");

                    TimeSpan startTime = TimeSpan.Parse(times[0]);
                    TimeSpan endTime = TimeSpan.Parse(times[1]);

                    string text = lines[i + 2];

                    Subtitle subtitle = new Subtitle(startTime, endTime, text, "");

                    subtitles.Add(subtitle);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading subtitles: " + ex.Message);
                subtitles.Clear();
            }

            return subtitles;
        }

        public void Save(string path, ICollection<Subtitle> subtitles)
        {
            var ordered = subtitles.OrderBy(s => s.ShowTime);
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    int i = 1;
                    foreach (Subtitle subtitle in ordered)
                    {
                        writer.WriteLine(i);
                        writer.WriteLine($"{subtitle.ShowTime.ToString("hh\\:mm\\:ss\\,fff")} --> {subtitle.HideTime.ToString("hh\\:mm\\:ss\\,fff")}");
                        writer.WriteLine(subtitle.Text);
                        writer.WriteLine();
                        ++i;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving subtitles: " + ex.Message);
            }
        }

        public void SaveTranslation(string path, ICollection<Subtitle> subtitles)
        {
            var ordered = subtitles.OrderBy(s => s.ShowTime);
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    int i = 1;
                    foreach (Subtitle subtitle in ordered)
                    {
                        writer.WriteLine(i);
                        writer.WriteLine($"{subtitle.ShowTime.ToString("hh\\:mm\\:ss\\,fff")} --> {subtitle.HideTime.ToString("hh\\:mm\\:ss\\,fff")}");
                        writer.WriteLine(subtitle.Translation);
                        writer.WriteLine();
                        ++i;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving subtitles: " + ex.Message);
            }
        }
    }
}
