using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Hamster.Plugin.Message;

namespace Hamster.Plugin
{
    public static class StringPatternHelper
    {
        private static Regex patternRegex = null;

        private static void Init()
        {
            if( patternRegex == null )
            {
                string formatPattern = "([^{}]+|}}|{{)*";
                string pattern = @"[^%]+|%(?<name>\w[-_\w\d]*)({(?<format>" +formatPattern+ ")}({(?<precision>" +formatPattern+ ")})?)?|(?<percent>%%)|%";
                Regex regex = new Regex( pattern, RegexOptions.Compiled|RegexOptions.Multiline );
                Interlocked.CompareExchange(ref patternRegex, regex, null);
            }
        }

        /// <summary>
        /// Liest Text von der Eingabe, ersetzt Platzhalter und schreibt das Ergebnis in die Ausgabe.
        /// </summary>
        /// <remarks>
        /// Siehe GetText für detailierte Informationen zum Format.
        /// </remarks>
        /// <param name="input">Eingabetext</param>
        /// <param name="writer">Ausgabetext</param>
        /// <param name="parameters">Zuordnung von Parameternamen zu Werten</param>
        public static void Transform( TextReader input, TextWriter writer, IDictionary<string, object> parameters )
        {
            string line;
            while( null != (line = input.ReadLine()) )
            {
                writer.WriteLine( GetText( line, parameters ) );
            }
        }

        /// <summary>
        /// Ersetzt Platzhalter in einem Text mit den entsprechenden Werten der Parameter.
        /// </summary>
        /// <remarks>
        /// Ein Platzhalter im Text wird mit einem %-Zeichen markiert. Folgendes Format wird verwendet:
        ///
        /// %name{format}{alignment}
        ///
        /// Die Werte alignment und format sind optional, aber wenn ein alignment angegeben wird, muss auch
        /// ein format angegeben werden. Es können leere Klammern verwendet werden, um das Standardformat
        /// zu verwenden.
        ///
        /// Die Werte für format und alignment werden an die Funktion string.Format übergeben. Dort kann
        /// eine Beschreibung der Bedeutung der Parameter nachgelesen werden.
        ///
        /// Der Name eines Platzhalters muss mit einem Buchstaben beginnen, gefolgt von Buchstaben, Ziffern
        /// sowie '-' und '_'. Wird ein anderes Zeichen gelesen, so hört der Name des Platzhalters dort auf.
        /// Beispielsweise in der Zeichenfolge "%hal{}lo %welt." sind die Platzhalter 'hal' und 'welt' enthalten.
        ///
        /// Bestimmte Zeichenfolgen beginnend mit % sind reserviert für Sonderzeichen:
        /// %% - wird ersetzt zu einem einzelnen %
        /// %n - wird mit einem Zeilenende ersetzt
        /// %t - wird mit einem Tab ersetzt
        /// %x{xx}{n} - wird ersetzt durch n Wiederholungen des Zeichens mit dem hexadezimalem Code xx
        /// </remarks>
        /// <param name="pattern">Text mit Platzhaltern</param>
        /// <param name="parameters">Zuordnung von Parameternamen zu Werten</param>
        /// <returns>Text mit eingesetzten Parametern</returns>
        public static string GetText( string pattern, IDictionary<string, object> parameters )
        {
            if(pattern == null || parameters == null || parameters.Count == 0)
                return pattern;

            Init();

            MatchEvaluator eval = delegate( Match m )
            {
                if( m.Groups["percent"].Success )
                {
                    return "%";
                }
                else if( m.Groups["name"].Success )
                {
                    string name = m.Groups["name"].Value;

                    switch( name )
                    {
                        case "n":
                            return Environment.NewLine;

                        case "t":
                            return "\t";

                        case "x":
                            if( !m.Groups["format"].Success || m.Groups["format"].Length == 0 )
                            {
                                return m.Value;
                            }

                            uint charVal;
                            if( !uint.TryParse( m.Groups["format"].Value, System.Globalization.NumberStyles.HexNumber, null, out charVal ) )
                            {
                                return m.Value;
                            }

                            int repeate = 1;
                            if( m.Groups["precision"].Success && m.Groups["precision"].Length > 0 )
                            {
                                if( !int.TryParse( m.Groups["format"].Value, out repeate ) )
                                {
                                    return m.Value;
                                }
                            }

                            return new string( (char)charVal, repeate );

                        default:
                            object value;
                            if( parameters.TryGetValue( name, out value ) )
                            {
                                string format = "";
                                if( m.Groups["precision"].Success && m.Groups["precision"].Length > 0 )
                                {
                                    format += "," + m.Groups["precision"];
                                }

                                if( m.Groups["format"].Success && m.Groups["format"].Length > 0 )
                                {
                                    format += ":" + m.Groups["format"];
                                }

                                return string.Format( "{0" + format + "}", value );
                            }
                            else
                            {
                                return m.Value;
                            }
                    }
                }
                else
                {
                    return m.Value;
                }
            };

            return patternRegex.Replace( pattern, eval );
        }

        public static string GetMessage(IMessageProvider messages, string messageId, IDictionary<string, object> parameters)
        {
            string id = messageId;
            string result = messages.GetMessage(id);
            int splitIndex;

            while (result == null && (splitIndex = id.IndexOf('/')) > 0)
            {
                id = id.Substring(splitIndex + 1);
                result = messages.GetMessage(id);
            }

            return StringPatternHelper.GetText(result, parameters);
        }

        public static string GetMessage(IMessageProvider messages, MessageException exception)
        {
            string result = GetMessage(messages, exception.MessageId, exception.Parameters);
            return result ?? exception.Message;
        }
    }
}
