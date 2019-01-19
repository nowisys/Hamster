using System;
using System.Reflection;
using System.IO;

namespace Hamster.Plugin.Configuration
{
    /// <summary>
    /// Interface das Services für die Administration verwaltet.
    /// </summary>
    public interface IAdminHost
    {
        /// <summary>
        /// Fügt einen Service hinzu.
        /// </summary>
        /// <param name="serviceType">Interface was vom Service angeboten wird.</param>
        /// <param name="path">Pfad unter dem der Service eingetragen werden soll.</param>
        /// <param name="service">Service der hinzugefügt werden soll.</param>
        void AddService(string path, object service, Type serviceType);

        /// <summary>
        /// Fügt einen Service hinzu.
        /// </summary>
        /// <typeparam name="T">Interface was vom Service angeboten wird.</typeparam>
        /// <param name="path">Pfad unter dem der Service eingetragen werden soll.</param>
        /// <param name="service">Service der hinzugefügt werden soll.</param>
        void AddService<T>(string path, T service);

        /// <summary>
        /// Fügt eine Embedded Resource für die Weboberfläche hinzu.
        /// </summary>
        /// <param name="path">Pfad an dem die Resource eingefügt wird.</param>
        /// <param name="data">Daten die an dem Pfad abrufbar sind.</param>
        void AddResource(string path, Stream data);

        /// <summary>
        /// Fügt einen Menüeintrag zur Administration hinzu.
        /// </summary>
        /// <param name="name">Text der im Menü angezeigt wird.</param>
        /// <param name="path">Pfad der aufgerufen wird, wenn der Menüeintrag ausgewählt wird.</param>
        void AddMenu(string name, string path);
    }
}
