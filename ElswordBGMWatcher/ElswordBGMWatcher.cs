using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ElswordBGMWatcher
{
    internal class ElswordBGMWatcher
    {
        public string currentPath { get => AppDomain.CurrentDomain.BaseDirectory; }
        public string streamPath { get => Path.Combine(this.currentPath, "current_bgm.txt"); }
        public string bgmPath { get; set; }
        public string activeBgm { get; set; }
        internal ElswordBGMWatcher(string bgmPath)
        {
            this.bgmPath = Path.GetFullPath(bgmPath);
            Console.WriteLine("Set bgm path: " + this.bgmPath);
            if (!Directory.Exists(this.bgmPath) || Directory.GetFiles(this.bgmPath).Where(x => !(new string[] { "ogg", "mp3" }.Contains(Path.GetExtension(x)))).Count() == 0)
            {
                throw new Exception("Invalid BGM Path");
            }
        }
        internal async Task Start()
        {
            await watchFiles();
        }
        public List<Process> GetProcessesAssociatedToFile(string file)
        {
            return Process.GetProcesses()
                .Where(x => !x.HasExited
                    && x.Modules.Cast<ProcessModule>().ToList()
                        .Exists(y => y.FileName.ToLowerInvariant() == file.ToLowerInvariant())
                    ).ToList();
        }
        void setActiveBGM(string file)
        {
            this.activeBgm = file;
            Console.WriteLine("Active BGM: " + Path.GetFileName(file));
            try
            {
                File.WriteAllText(this.streamPath, Path.GetFileName(this.activeBgm));
            } catch (IOException)
            {
                Console.Error.WriteLine("Failed writing current active bgm to disk");
            }
        }
        internal async Task watchFiles()
        {
            var t = new Timer(100);
            t.Elapsed += (s, e) =>
            {
                t.Stop();
                t.Interval = 2500;
                var files = Directory.GetFiles(this.bgmPath, "*.ogg");
                string activeFile = null;
                foreach(var f in files)
                {
                    if (FileUtil.WhoIsLocking(f).Where(x => x.ProcessName == "x2").Count() > 0)
                    {
                        activeFile = f;
                    }
                }
                if (activeFile != null && this.activeBgm != activeFile)
                {
                    this.setActiveBGM(activeFile);
                }
                t.Start();
            };
            t.Start();
            await Task.Delay(-1);
        }
    }
}
