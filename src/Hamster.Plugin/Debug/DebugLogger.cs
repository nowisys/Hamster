using System;
using System.Text;
using System.Collections.Generic;

namespace Hamster.Plugin.Debug
{
    public class DebugLogger : ILogger
    {
        private LogLevel level;
        private string name;
        private Dictionary<LogLevel,Tuple<ConsoleColor, ConsoleColor>> colorMap;

        public DebugLogger( string name )
            : this(name, LogLevel.Debug)
        {

        }

        public DebugLogger( string name, LogLevel level )
        {
            this.name = name;
            this.level = level;
            this.colorMap = new Dictionary<LogLevel,Tuple<ConsoleColor, ConsoleColor>> {
                { LogLevel.Debug, Tuple.Create(ConsoleColor.Blue, ConsoleColor.Black) },
                { LogLevel.Info, Tuple.Create(ConsoleColor.Green, ConsoleColor.Black) },
                { LogLevel.Warn, Tuple.Create(ConsoleColor.Yellow, ConsoleColor.Black) },
                { LogLevel.Error, Tuple.Create(ConsoleColor.Red, ConsoleColor.Black) },
                { LogLevel.Fatal, Tuple.Create(ConsoleColor.Black, ConsoleColor.Red) },
            };
        }

        public virtual bool IsLevelEnabled(LogLevel level)
        {
            return level <= this.level;
        }

        protected virtual string FormatMessage( IFormatProvider formatProvider, Exception x, string format, params object[] args )
        {
            var result = new StringBuilder();
            if (args == null || args.Length == 0) {
                result.Append(format);
            } else {
                result.AppendFormat(formatProvider, format, args);
            }

            if( x != null )
            {
                result.Append(Environment.NewLine);
                result.Append(x);
            }
            return result.ToString();
        }

        public virtual void WriteMessage(LogLevel level, IFormatProvider formatProvider, string format, params object[] args )
        {
            string message = FormatMessage( formatProvider, null, format, args );
            WriteMessage( level, message );
        }

        public virtual void WriteException(LogLevel level, Exception x, IFormatProvider formatProvider, string format, params object[] args )
        {
            string message = FormatMessage( formatProvider, x, format, args );
            WriteMessage( level, message );
        }

        protected virtual void WriteMessage(LogLevel level, string text)
        {
            Tuple<ConsoleColor, ConsoleColor> colors;
            if (!colorMap.TryGetValue(level, out colors)) {
                colors = Tuple.Create(ConsoleColor.White, ConsoleColor.Black);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("{0:s} ", DateTime.Now);

            Console.ForegroundColor = colors.Item1;
            Console.BackgroundColor = colors.Item2;
            Console.Write(level.ToString());

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(" {0} - ", name);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
        }

        public virtual ILogger CreateChildLogger(string name)
        {
            return new DebugLogger( this.name + "." + name, level );
        }
    }
}
