using System;

namespace Hamster.Plugin.Data
{
    /// <summary>
    /// Abstrakte Klasse die überladene Methoden von IDataStore vordefiniert.
    /// </summary>
    public abstract class AbstractDataStore : IDataStore
    {
        /// <summary>
        /// Speichert ein Element dauerhaft ab.
        /// </summary>
        /// <param name="handlerName">Bezeichner unter dem das Objekt abgelegt wird.</param>
        /// <param name="data">XmlElement das gespeichert wird.</param>
        /// <param name="dataType">String der die Art der Daten spezifiziert.</param>
        /// <returns>Integer der das Objekt eindeutig identifiziert.</returns>
        public abstract int Store( string handlerName, System.Xml.XmlElement data, string dataType );

        /// <summary>
        /// Setzt den Fehlerstatus für ein gespeichertes Objekt.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <param name="message">Nachricht mit der der Fehler gesetzt wird.</param>
        /// <returns>True wenn der Status erfolgreich gesetzt wurde; false sonst.</returns>
        public abstract bool SetError( int dataId, string message );

        /// <summary>
        /// Setzt den Status des Objekts auf Erfolg.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <returns>True wenn der Status erfolgreich gesetzt wurde; false sonst.</returns>
        public abstract bool SetSuccess( int dataId );

        /// <summary>
        /// Löscht ein Objekt aus dem Speicher.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        public abstract void Delete( int dataId );

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        public virtual void Delete( string handlerName )
        {
            Delete( handlerName, DataStoreItemType.All, DateTime.Now );
        }

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        public virtual void Delete( string handlerName, DataStoreItemType type )
        {
            Delete( handlerName, type, DateTime.Now );
        }

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        /// <param name="maxDate">Datum bis zu dem alle zu löschenden Objekte erstellt wurden.</param>
        public abstract void Delete( string handlerName, DataStoreItemType type, DateTime maxDate );

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        public virtual void DeleteAll()
        {
            DeleteAll( DataStoreItemType.All, DateTime.Now );
        }

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        public virtual void DeleteAll( DataStoreItemType type )
        {
            DeleteAll( type, DateTime.Now );
        }

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        /// <param name="maxDate">Datum bis zu dem alle zu löschenden Objekte erstellt wurden.</param>
        public abstract void DeleteAll( DataStoreItemType type, DateTime maxDate );

        /// <summary>
        /// Liefert einen bestimmten Eintrag aus dem Speicher.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <returns>Das gesuchte Objekt oder null wenn es nicht existiert.</returns>
        public abstract DataStoreItem GetItem( int dataId );

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        public virtual DataStoreItem[] GetItems()
        {
            return GetItems( DataStoreItemType.All );
        }

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="type">Zustände der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        public abstract DataStoreItem[] GetItems( DataStoreItemType type );

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="handlerName">Bezeichner der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        public virtual DataStoreItem[] GetItems( string target )
        {
            return GetItems( target, DataStoreItemType.All );
        }

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="handlerName">Bezeichner der gesuchten Objekte.</param>
        /// <param name="type">Zustände der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        public abstract DataStoreItem[] GetItems( string handlerName, DataStoreItemType type );
    }
}
