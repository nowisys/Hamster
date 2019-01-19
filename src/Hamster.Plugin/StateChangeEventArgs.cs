namespace Hamster.Plugin
{
    public class StateChangeEventArgs
    {
        public IPlugin Plugin { get; }
        public PluginState OldState { get; }
        public PluginState NewState { get; }

        public StateChangeEventArgs(IPlugin plugin, PluginState oldState, PluginState newState)
        {
            this.Plugin = plugin;
            this.OldState = oldState;
            this.NewState = newState;
        }

        public bool Cancelled { get; private set; }

        public void Cancel() {
            Cancelled = true;
        }
    }
}
