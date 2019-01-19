using System;
using System.Xml;

namespace Hamster.Plugin.Data
{
    /// <summary>
    /// Ein Interface zum Zugriff auf einen persistenten Speicher für XML.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Speichert ein Element dauerhaft ab.
        /// </summary>
        /// <param name="handlerName">Bezeichner unter dem das Objekt abgelegt wird.</param>
        /// <param name="data">XmlElement das gespeichert wird.</param>
        /// <param name="dataType">String der die Art der Daten spezifiziert.</param>
        /// <returns>Integer der das Objekt eindeutig identifiziert.</returns>
        int Store( string handlerName, XmlElement data, string dataType );

        /// <summary>
        /// Setzt den Fehlerstatus für ein gespeichertes Objekt.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <param name="message">Nachricht mit der der Fehler gesetzt wird.</param>
        /// <returns>True wenn der Status erfolgreich gesetzt wurde; false sonst.</returns>
        bool SetError( int dataId, string message );

        /// <summary>
        /// Setzt den Status des Objekts auf Erfolg.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <returns>True wenn der Status erfolgreich gesetzt wurde; false sonst.</returns>
        bool SetSuccess( int dataId );

        /// <summary>
        /// Löscht ein Objekt aus dem Speicher.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        void Delete( int dataId );

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        void Delete( string handlerName );

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        void Delete( string handlerName, DataStoreItemType type );

        /// <summary>
        /// Löscht alle Objekte zu einem bestimmten Bezeichner.
        /// </summary>
        /// <param name="handlerName">Bezeichner zu dem alle Objekte gelöscht werden.</param>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        /// <param name="maxDate">Datum bis zu dem alle zu löschenden Objekte erstellt wurden.</param>
        void Delete( string handlerName, DataStoreItemType type, DateTime maxDate );

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        void DeleteAll( DataStoreItemType type );

        /// <summary>
        /// Löscht alle Objekte im Speicher.
        /// </summary>
        /// <param name="type">Zustände der Objekte die gelöscht werden.</param>
        /// <param name="maxDate">Datum bis zu dem alle zu löschenden Objekte erstellt wurden.</param>
        void DeleteAll( DataStoreItemType type, DateTime maxDate );

        /// <summary>
        /// Liefert einen bestimmten Eintrag aus dem Speicher.
        /// </summary>
        /// <param name="dataId">Integer der das Objekt eindeutig identifiziert.</param>
        /// <returns>Das gesuchte Objekt oder null wenn es nicht existiert.</returns>
        DataStoreItem GetItem( int dataId );

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        DataStoreItem[] GetItems();

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="type">Zustände der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        DataStoreItem[] GetItems( DataStoreItemType type );

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="handlerName">Bezeichner der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        DataStoreItem[] GetItems( string handlerName );

        /// <summary>
        /// Liefert alle Einträge im Speicher.
        /// </summary>
        /// <param name="handlerName">Bezeichner der gesuchten Objekte.</param>
        /// <param name="type">Zustände der gesuchten Objekte.</param>
        /// <returns>Array mit Einträgen aus dem Speicher.</returns>
        DataStoreItem[] GetItems( string handlerName, DataStoreItemType type );
    }
}
