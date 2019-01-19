using System;
using System.Xml;

namespace Hamster.Plugin.Data
{
    /// <summary>
    /// Repräsentiert einen Eintrag in einem DataStore.
    /// </summary>
    [Serializable()]
    public class DataStoreItem
    {
        private int dataId;
        private string handlerName;
        private DateTime creationTime;
        private string error;
        private DateTime? processTime;
        private XmlElement data;
        private string dataType;

        /// <summary>
        /// Zeigt und setzt einen Integer, der den Eintrag eindeutig im DataStore identifiziert.
        /// </summary>
        public int DataId
        {
            get { return dataId; }
            set { dataId = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Bezeichner unter dem der Eintrag angelegt wurde.
        /// </summary>
        public string HandlerName
        {
            get { return handlerName; }
            set { handlerName = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Zeitpunkt zu dem der Eintrag erstellt wurde.
        /// </summary>
        public DateTime CreationTime
        {
            get { return creationTime; }
            set { creationTime = value; }
        }

        /// <summary>
        /// Zeigt und setzt den Fehler der für den Eintrag gesetzt wurde.
        /// </summary>
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        /// <summary>
        /// Zeigt und setzt das Datum der letzten Verarbeitung.
        /// </summary>
        public DateTime? ProcessTime
        {
            get { return processTime; }
            set { processTime = value; }
        }

        /// <summary>
        /// Zeigt und setzt die gespeicherten Daten.
        /// </summary>
        public XmlElement Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Zeigt und setzt einen String der die Art der Daten definiert.
        /// </summary>
        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
    }
}
