using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleComposer
{
    public interface ISubtitlesPlugin
    {
        string Name { get; }
        string Extension { get; }
        ICollection<Subtitle> Load(string path);
        void Save(string path, ICollection<Subtitle> subtitles);
        void SaveTranslation(string path, ICollection<Subtitle> subtitles);
    }
}
