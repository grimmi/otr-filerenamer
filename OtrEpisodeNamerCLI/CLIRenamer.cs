using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EpisodeNamer;
using FileDistributor;
using OtrBatchDecoder;
using SQLite;
using WikipediaShowCrawler;

namespace OtrEpisodeNamerCLI
{
    public class CLIRenamer
    {
        private static CommandLineWindow window = new CommandLineWindow();
        private static readonly Dictionary<string, string> showMapping = new Dictionary<string, string>();
        private static Configuration config;
        private static KeyValueConfigurationCollection settings;

        private static SQLiteConnection db;
        private static IEnumerable<ShowMapping> mappings;

        [STAThread]
        public static void Execute(string[] args)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dbPath = Path.Combine(path, "showmappings.db3");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<ShowMapping>();
            mappings = db.Table<ShowMapping>();
            InitConfig();
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var startDir = args != null && args.Length > 0
                ? args[0]
                : @"";

            var targetDir = args != null && args.Length > 1
                ? args[1]
                : @"";
            try
            {
                if (string.IsNullOrWhiteSpace(startDir))
                {
                    using (var dirDialog = new FolderBrowserDialog())
                    {
                        SetPathFromConfig(dirDialog, "startdir");
                        dirDialog.Description = "Ausgangspfad auswählen";
                        var dlgOk = dirDialog.ShowDialog(window);
                        if (dlgOk == DialogResult.OK)
                        {
                            startDir = dirDialog.SelectedPath;
                        }
                        else
                        {
                            Console.WriteLine("kein Pfad angegeben");
                            Environment.Exit(1);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(targetDir))
                {
                    using (var dirDialog = new FolderBrowserDialog())
                    {
                        SetPathFromConfig(dirDialog, "targetdir");
                        dirDialog.Description = "Zielpfad angeben";
                        var dlgOk = dirDialog.ShowDialog(window);
                        if (dlgOk == DialogResult.OK)
                        {
                            targetDir = dirDialog.SelectedPath;
                        }
                        else
                        {
                            Console.WriteLine("kein Pfad angegeben");
                            Environment.Exit(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }


            var decodedDir = Path.Combine(startDir, "decoded");
            SetConfigValue("startdir", decodedDir);
            SetConfigValue("targetdir", targetDir);

            var decoder = new OtrBatchDecoder.OtrBatchDecoder();
            var decodedFiles = decoder.Decode(new DecoderOptions
            {
                AutoCut = true,
                ContinueWithoutCutlist = true,
                CreateDirectories = true,
                DecoderPath = GetConfigValue("decoderpath"),
                Email = GetConfigValue("email"),
                FileExtensions = new[] {".otrkey"},
                ForceOverwrite = true,
                InputDirectory = startDir,
                OutputDirectory = Path.Combine(startDir, "decoded"),
                Password = GetConfigValue("password")
            }).ToList();

            await Run(decodedFiles, targetDir);
            SetConfigValue("startdir", startDir);
            
            foreach (var f in decodedFiles)
            {
                File.Delete(f);
            }
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
                config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
                settings = config.AppSettings.Settings;
            }
        }

        private static async Task Run(IEnumerable<string> files, string targetDir)
        {
            var showNameParser = new OtrShowNameParser();
            var showFiles = new Dictionary<string, Dictionary<string, string>>();
            foreach (var f in files)
            {
                try
                {
                    var showName = showNameParser.GetShowName(f);
                    showName = AskUserForShowName(showName, f);

                    //var renamer = new SingleFileRenamer(f, showName, new WikipediaCrawler(), new OtrEpisodeFinder());
                    var renamer = new SingleFileRenamer(f, showName, new Crawler.TvDbCrawler(), new OtrEpisodeFinder());
                    var newName = await renamer.RenameFile("");
                    Console.WriteLine("Neuer Name: " + newName);
                    Console.WriteLine("---------------------------------------------------");

                    if (!showFiles.ContainsKey(showName))
                    {
                        showFiles.Add(showName, new Dictionary<string, string>());
                    }
                    showFiles[showName][f] = newName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler: " + ex);
                }
            }

            foreach (var kvp in showFiles)
            {
                var show = kvp.Key;
                var srcToTarget = kvp.Value;
                var distributor = new FileCopier(targetDir);
                distributor.DistributeFiles(show, srcToTarget);
            }

            Console.WriteLine("Programm abgeschlossen");
        }

        private static string AskUserForShowName(string showName, string file)
        {
            Console.WriteLine("Datei analysiert: " + file);

            var savedMapping = mappings.FirstOrDefault(m => m.FileShowName == showName);
            if (savedMapping != null)
            {
                Console.WriteLine("Show aus Datenbank geladen: " + savedMapping.UserShowName);
                return savedMapping.UserShowName;
            }
            Console.WriteLine("Show erkannt: " + showName);
            Console.WriteLine("Name richtig? [j]/n");

            var response = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(response) || response.ToLower() == "j")
            {
                if (!showMapping.ContainsKey(showName))
                {
                    db.Insert(new ShowMapping { FileShowName = showName, UserShowName = showName });
                }
                return showName;
            }
            else
            {
                Console.WriteLine("ShowName eingeben: ");
                var userShowName = Console.ReadLine();
                var userMapping = new ShowMapping { FileShowName = showName, UserShowName = userShowName };
                db.Insert(userMapping);
                return userShowName;
            }
        }
    }

    internal class CommandLineWindow : IWin32Window
    {
        #region IWin32Window Members
        public IntPtr Handle
        {
            get
            {
                return (Process.GetCurrentProcess().MainWindowHandle);
            }
        }
        #endregion
    }
}
