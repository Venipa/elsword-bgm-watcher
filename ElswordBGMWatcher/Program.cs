using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElswordBGMWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser();
            p.Setup<string>("bgm")
             .Callback(path =>
            new ElswordBGMWatcher(Path.GetFullPath(path)).Start().Wait())
             .WithDescription("Set BGM Path on which to watch for")
             .Required();
            p.SetupHelp("h", "?", "help")
                .Callback(text => Console.WriteLine(text));
            p.HelpOption.ShowHelp(p.Options);
            p.Parse(args);
        }
    }
}
