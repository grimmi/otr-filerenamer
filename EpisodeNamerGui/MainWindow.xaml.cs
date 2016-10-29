using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EpisodeNamer;
using PropertyChanged;
using WikipediaShowCrawler;
using WinForms = System.Windows.Forms;

namespace EpisodeNamerGui
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    [ImplementPropertyChanged]
    public partial class MainWindow : Window
    {
        public MainViewModel Model { get; set; }

        public bool CanReadEpisodes { get; set; }

        public bool CanProcessEpisodes { get; set; }
        private static Configuration config;
        private static KeyValueConfigurationCollection settings;

        public MainWindow()
        {
            Model = new MainViewModel();
            InitConfig();
            InitializeComponent();

            LoadConfigValues();
        }

        private void LoadConfigValues()
        {
            Model.SourceDirectory = GetConfigValue("sourcedir");
            Model.TargetDirectory = GetConfigValue("targetdir");
        }

        private static void SetPathFromConfig(FolderBrowserDialog dlg, string cfgKey)
        {
            var path = GetConfigValue(cfgKey);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            while (!Directory.Exists(path) && Directory.GetParent(path) != null)
            {
                var parent = Directory.GetParent(path);
                path = parent.FullName;
            }

            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                dlg.SelectedPath = path;
            }
        }

        private static string GetConfigValue(string key)
        {
            if (settings.AllKeys.Contains(key))
            {
                return settings[key].Value;
            }
            return string.Empty;
        }

        private static void SetConfigValue(string key, string value)
        {
            if (!settings.AllKeys.Contains(key))
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            config.Save(ConfigurationSaveMode.Full, true);
        }

        private static void InitConfig()
        {
            if (config == null)
            {
                config = ConfigurationManager.OpenExeConfiguration(WinForms.Application.ExecutablePath);
                settings = config.AppSettings.Settings;
            }
        }

        private void SourceDirButtonClicked(object sender, RoutedEventArgs e)
        {
            SetPath(p => Model.SourceDirectory = p, "sourcedir");
            SetConfigValue("sourcedir", Model.SourceDirectory);
            CanReadEpisodes = !string.IsNullOrWhiteSpace(Model.SourceDirectory) && Directory.Exists(Model.SourceDirectory);
        }

        private void TargetDirButtonClicked(object sender, RoutedEventArgs e)
        {
            SetPath(p => Model.TargetDirectory = p, "targetdir");
            SetConfigValue("targetdir", Model.TargetDirectory);
            CanProcessEpisodes = EpisodeList.Episodes.Any() && !string.IsNullOrWhiteSpace(Model.TargetDirectory);
        }

        private void SetPath(Action<string> pathSetter, string configKey = "")
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(configKey))
                {
                    SetPathFromConfig(folderDialog, configKey);
                }

                var dialogResult = folderDialog.ShowDialog();
                if (dialogResult == WinForms.DialogResult.OK)
                {
                    pathSetter(folderDialog.SelectedPath);
                }
            }
        }

        private async void GetEpisodes(object sender, RoutedEventArgs e)
        {
            var movieFiles = Directory.EnumerateFiles(Model.SourceDirectory, "*.avi");
            var episodeFiles = new List<EpisodeFile>();
            foreach (var file in movieFiles)
            {
                var showNameParser = new OtrShowNameParser();
                var finder = new OtrEpisodeFinder();
                var crawler = new WikipediaCrawler();

                var showName = showNameParser.GetShowName(file);
                var episode = await finder.GetEpisodeAsync(file, showName, crawler);

                var episodeFile = new EpisodeFile { Episode = episode.Episode, File = file };
                episodeFiles.Add(episodeFile);
            }

            EpisodeList.Episodes = new ObservableCollection<EpisodeViewModel>(episodeFiles.Select(ep => new EpisodeViewModel(ep)));
        }

        private void SourceTextChanged(object sender, TextCompositionEventArgs e)
        {
            CanReadEpisodes = !string.IsNullOrWhiteSpace(Model.SourceDirectory) && Directory.Exists(Model.SourceDirectory);
        }

        private void TargetTextChanged(object sender, TextCompositionEventArgs e)
        {
            CanProcessEpisodes = EpisodeList.Episodes.Any() && !string.IsNullOrWhiteSpace(Model.TargetDirectory);
        }
    }
}
