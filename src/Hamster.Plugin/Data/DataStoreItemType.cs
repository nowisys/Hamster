using System;

namespace Hamster.Plugin.Data
{
    /// <summary>
    /// Flags zur genaueren spezifikation von DataStore Einträgen.
    /// </summary>
    [Flags()]
    public enum DataStoreItemType
    {
        /// <summary>
        /// Spezifiziert unbearbeitete Einträge.
        /// </summary>
        Untouched = 1,
        /// <summary>
        /// Spezifiziert Einträge mit Fehlerstatus.
        /// </summary>
        Error = 2,
        /// <summary>
        /// Spezifiziert Einträge mit Erfolgsstatus.
        /// </summary>
        Success = 4,

        /// <summary>
        /// Alle Einträge.
        /// </summary>
        All = 7
    }
}
