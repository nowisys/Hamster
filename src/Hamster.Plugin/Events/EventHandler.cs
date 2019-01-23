using System;

namespace Hamster.Plugin.Events
{
    /// <summary>
    /// Repräsentiert Methoden die zu einem Event ausgelöst werden.
    /// </summary>
    /// <typeparam name="TSender">Klasse des Auslösers des Events.</typeparam>
    /// <typeparam name="TArgs">Klasse der Daten des Events.</typeparam>
    /// <param name="sender">Auslöser des Events.</param>
    /// <param name="args">Daten des Events.</param>
    public delegate void EventHandler<TSender, TArgs>(TSender sender, TArgs args);
}
