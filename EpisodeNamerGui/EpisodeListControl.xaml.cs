using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EpisodeNamer;
using PropertyChanged;

namespace EpisodeNamerGui
{
    /// <summary>
    /// Interaktionslogik für TheList.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class EpisodeListControl : UserControl
    {
        public ObservableCollection<EpisodeViewModel> Episodes { get; set; }
        private bool groupInitDone = false;

        public EpisodeListControl()
        {
            InitializeComponent();
        }
    }
}
