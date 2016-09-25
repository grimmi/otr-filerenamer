﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EpisodeNamer;
using FileDistributor;
using WikipediaShowCrawler;

namespace OtrEpisodeNamerCLI
{
    class Program
    {
        private static CommandLineWindow window = new CommandLineWindow();
        private static readonly Dictionary<string, string> showMapping = new Dictionary<string, string>();

        [STAThread()]
        static void Main(string[] args)
        {
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

            var files = Directory.EnumerateFiles(startDir, "*.mpg.avi", SearchOption.AllDirectories);

            await Run(files, targetDir);
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

                    var renamer = new SingleFileRenamer(f, showName, new WikipediaCrawler(), new OtrEpisodeFinder());
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
            if (showMapping.ContainsKey(showName))
            {
                showName = showMapping[showName];
                Console.WriteLine("Show bereits bestätigt: " + showName);
                return showName;
            }
            Console.WriteLine("Show erkannt: " + showName);
            Console.WriteLine("Name richtig? [j]/n");

            var response = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(response) || response.ToLower() == "j")
            {
                if (!showMapping.ContainsKey(showName))
                {
                    showMapping.Add(showName, showName);
                }
                return showName;
            }
            else
            {
                Console.WriteLine("ShowName eingeben: ");
                var userShowName = Console.ReadLine();
                if (!showMapping.ContainsKey(userShowName))
                {
                    showMapping.Add(showName, userShowName);
                }
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
