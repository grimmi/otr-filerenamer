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

RunTarget(target);