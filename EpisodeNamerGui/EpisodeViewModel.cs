using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EpisodeNamer;
using TvShowManager;

namespace EpisodeNamerGui
{
    public class EpisodeViewModel
    {
        public string EpisodeName { get; }
        public int EpisodeNumber { get; }
        public string ShowName { get; }
        public int SeasonNumber { get; }
        public string FileName { get; }

        public EpisodeFile EpisodeFile { get; }

        public EpisodeViewModel(EpisodeFile episodeFile)
        {
            EpisodeName = episodeFile.Episode.Name;
            EpisodeNumber = episodeFile.Episode.Number;
            ShowName = episodeFile.Episode.Season.ShowName;
            SeasonNumber = episodeFile.Episode.Season.Number;
            FileName = episodeFile.File;

            EpisodeFile = episodeFile;
        }
    }
}
