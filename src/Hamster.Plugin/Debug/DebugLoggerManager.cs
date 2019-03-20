using System;
using System.Collections.Generic;

namespace Hamster.Plugin.Debug
{
    public class DebugLoggerManager : ILoggerManager
    {
        private List<LogEventArgs> entries = new List<LogEventArgs>();
        private ILogger logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public event EventHandler<LogEventArgs> MessageLogged;

        protected virtual void OnMessageLogged(LogEventArgs args)
        {
            EventHandler<LogEventArgs> handler = MessageLogged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void AddEntry(DateTime time, string level, string logName, string message, string exceptionText)
        {
            logger.Info("AddEntry( {0}, {1}, {2}, {3} )", logName, message, time, level);
            LogEventArgs args = new LogEventArgs(entries.Count, time, level, logName, message, exceptionText);
            entries.Add(args);
            OnMessageLogged(args);
        }

        public IList<LogEventArgs> GetEntries()
        {
            return entries.AsReadOnly();
        }

        public ILogger GetLogger(string name)
        {
            return Logger.CreateChildLogger(name);
        }
    }
}
