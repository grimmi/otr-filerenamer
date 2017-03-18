var target = Argument("target", "Default");
var configuration = Argument("Configuration", "Debug");

Task("nuget-restore")
    .Does(() =>
    {
        NuGetRestore("EpisodeNamer.sln");
    });

Task("rebuild")
    .IsDependentOn("nuget-restore")
    .Does(() =>
    {        
        var settings = new MSBuildSettings();
        settings.Configuration = configuration;
        settings.Targets.Add("Clean");
        settings.Targets.Add("Rebuild");
        settings.PlatformTarget = PlatformTarget.MSIL;

        MSBuild("./EpisodeNamer.sln", settings);
    });

Task("Default")
    .IsDependentOn("rebuild")
    .Does(() =>
    {
        Information("in default");
        var otrCliDir = "./OtrEpisodeNamerCLI/bin/" + configuration + "/";
        var libDir = otrCliDir + "lib";
        if(DirectoryExists(libDir))
        {
            DeleteDirectory(libDir, true);
        }
        CreateDirectory(libDir);
        var dllfiles = GetFiles(otrCliDir + "*.dll");
        foreach(var f in dllfiles)
        {
            MoveFileToDirectory(f, libDir);
            Information("moved file {0}", f);
        }

        var pdbFiles = GetFiles(otrCliDir + "*.pdb");
        DeleteFiles(pdbFiles);

        var xmlFiles = GetFiles(otrCliDir + "*.xml");
        DeleteFiles(xmlFiles);

        var vshostexe = otrCliDir + "OtrEpisodeNamerCLI.vshost.exe"; 
        if(FileExists(vshostexe))
        {
            DeleteFile(vshostexe);
        }

        var vshostcfg = otrCliDir + "OtrEpisodeNamerCLI.vshost.exe.config";
        if(FileExists(vshostcfg))
        {
            DeleteFile(vshostcfg);
        }

        var vshostmanifest = otrCliDir + "OtrEpisodeNamerCLI.vshost.exe.manifest";
        if(FileExists(vshostmanifest))
        {
            DeleteFile(vshostmanifest);
        }
    });

Task("zip")
    .IsDependentOn("Default")
    .Does(() =>
    {
        var otrCliDir = "./OtrEpisodeNamerCLI/bin/" + configuration + "/";
        var zipFile = otrCliDir + "Renamer.zip";
        Zip(otrCliDir, zipFile);
    });

Task("nuget-pack")
    .IsDependentOn("Default")
    .Does(() =>
    {
        var outDirectory = "./nuget";
        if(!DirectoryExists(outDirectory))
        {
            CreateDirectory(outDirectory);            
        }
        var nugetSettings = new NuGetPackSettings
        {
            Id                  =       "OtrRenamer",
            Version             =       "0.0.0.3",
            Title               =       "OTR Decoder, Renamer and Distributor",
            Authors             =       new[]{"Arne Lehmann"},
            Description         =       "Tool to decode, name and organize episodes downloaded from onlinetvrecoder.com",
            ProjectUrl          =       new Uri("http://example.com"),
            LicenseUrl          =       new Uri("http://example.com"),
            Copyright           =       "Arne Lehmann 2016",
            Files               =       new[]
                                        {
                                            new NuSpecContent { Source = "OtrEpisodeNamerCLI.exe" },
                                            new NuSpecContent { Source = "OtrEpisodeNamerCLI.exe.config" },
                                            new NuSpecContent { Source = "lib/EpisodeNamer.dll", Target = "lib/EpisodeNamer.dll" },
                                            new NuSpecContent { Source = "lib/FileDistributor.dll", Target = "lib/FileDistributor.dll" },
                                            new NuSpecContent { Source = "lib/HtmlAgilityPack.dll", Target = "lib/HtmlAgilityPack.dll" },
                                            new NuSpecContent { Source = "lib/OtrBatchDecoder.dll", Target = "lib/OtrBatchDecoder.dll" },
                                            new NuSpecContent { Source = "lib/SQLite-net.dll", Target = "lib/SQLite-net.dll" },
                                            new NuSpecContent { Source = "lib/SQLitePCLRaw.batteries_green.dll", Target = "lib/SQLitePCLRaw.batteries_green.dll" },
                                            new NuSpecContent { Source = "lib/SQLitePCLRaw.core.dll", Target = "lib/SQLitePCLRaw.core.dll" },
                                            new NuSpecContent { Source = "lib/SQLitePCLRaw.provider.e_sqlite3.dll", Target = "lib/SQLitePCLRaw.provider.e_sqlite3.dll" },
                                            new NuSpecContent { Source = "lib/TvShowManager.dll", Target = "lib/TvShowManager.dll" },
                                            new NuSpecContent { Source = "lib/WikipediaShowCrawler.dll", Target = "lib/WikipediaShowCrawler.dll" },
                                            new NuSpecContent { Source = "x64/e_sqlite3.dll", Target = "x64/e_sqlite3.dll" },
                                            new NuSpecContent { Source = "x86/e_sqlite3.dll", Target = "x86/e_sqlite3.dll" }
                                        },   
            BasePath            =       "./OtrEpisodeNamerCLI/bin/" + configuration + "/",
            OutputDirectory     =       "./nuget"         
        };

        NuGetPack(nugetSettings);
    });

RunTarget(target);