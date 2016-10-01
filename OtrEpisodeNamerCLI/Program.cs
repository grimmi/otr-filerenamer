using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OtrEpisodeNamerCLI
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
            {
                var name = eventArgs.Name.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)[0];

                var libPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                                                                    "lib",
                                                                    name + ".dll");
                return Assembly.LoadFile(libPath);
            };
            CLIRenamer.Execute(args);
        }
    }
}
