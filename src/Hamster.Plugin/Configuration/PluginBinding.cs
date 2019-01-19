using System;

namespace Hamster.Plugin.Configuration
{
    [Serializable()]
    public class PluginBinding
    {
        private string targetPlugin;
        private string nativeName;
        private string boundPlugin;

        public PluginBinding()
        {

        }

        public string TargetPlugin
        {
            get { return targetPlugin; }
            set { targetPlugin = value; }
        }

        public string NativeName
        {
            get { return nativeName; }
            set { nativeName = value; }
        }

        public string BoundPlugin
        {
            get { return boundPlugin; }
            set { boundPlugin = value; }
        }

        public override bool Equals( object obj )
        {
            PluginBinding other = obj as PluginBinding;
            return other != null
                && string.Equals( targetPlugin, other.targetPlugin )
                && string.Equals( nativeName, other.nativeName );
        }

        public override int GetHashCode()
        {
            int result = targetPlugin != null ? targetPlugin.GetHashCode() : 0;
            result ^= nativeName != null ? nativeName.GetHashCode() : 0;
            return result;
        }
    }
}
