using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EpisodeNamer;
using PropertyChanged;

namespace EpisodeNamerGui
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        public string SourceDirectory { get; set; } = "Verzeichnis wählen";
        public string TargetDirectory { get; set; } = "Verzeichnis wählen";
    }
}
