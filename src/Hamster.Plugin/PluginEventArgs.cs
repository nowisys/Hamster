using System;
using System.Collections.Generic;
using System.Text;

namespace Hamster.Plugin
{
    public class PluginEventArgs : EventArgs
    {
        public PluginEventArgs(IPlugin plugin)
        {
            this.Plugin = plugin;
        }

        public IPlugin Plugin { get; private set; }
    }
}
