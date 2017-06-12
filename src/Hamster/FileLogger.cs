using System;
using System.IO;
using Hamster.Plugin;
using Hamster.Plugin.Debug;

namespace Hamster
{
    public class FileLogger : DebugLogger
    {
        private string directory;
        private string path;
        private string name;
        private LogLevel level;

        public FileLogger(string directory, string name)
            : this(directory, name, LogLevel.Info)
        {
        }

        public FileLogger(string directory, string name, LogLevel level)
            : base(name, level)
        {
            this.directory = directory;
            this.name = name;
            this.level = level;
            this.path = Path.Combine(directory, name + ".log");
        }

        public override ILogger CreateChildLogger(string name)
        {
            return new FileLogger(this.directory, this.name + "." + name, this.level);
        }

        protected override void WriteMessage(LogLevel level, string text)
        {
            string fullText = String.Format("{0:s} {1} {2} - {3}\n", DateTime.Now, level, name, text);
            File.AppendAllText(this.path, fullText);
        }
    }
}
