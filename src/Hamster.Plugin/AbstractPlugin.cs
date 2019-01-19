using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Hamster.Plugin.Configuration;
using Hamster.Plugin.IoC;

namespace Hamster.Plugin
{
    public abstract class AbstractPlugin<TSettings> : IPlugin, IXmlConfigurable, IDisposable
        where TSettings : new()
    {
        private string name;
        private bool isConfigured;
        private bool isInitalized;
        private TSettings settings;
        private Type settingsType;

        private object stateSync = new object();
        private bool isOpen;

        private ILogger logger = NullLogger.Instance;
        private IPluginServiceProvider provider;

        public AbstractPlugin()
            : this( typeof(TSettings) )
        {

        }

        public AbstractPlugin( Type settingsType )
        {
            this.settingsType = settingsType;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public ILogger Logger
        {
            get { return logger; }
            set { logger = value ?? NullLogger.Instance; }
        }

        public IPluginServiceProvider PluginServiceProvider
        {
            get { return provider; }
            set { provider = value; }
        }

        public TSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public bool IsConfigured
        {
            get { return isConfigured; }
            protected set { isConfigured = value; }
        }

        public bool IsOpen
        {
            get { return isOpen; }
            protected set { isOpen = value; }
        }

        public virtual bool IsReady
        {
            get { return IsOpen; }
        }

        protected bool IsInitalized
        {
            get { return isInitalized; }
            set { isInitalized = value; }
        }

        public event EventHandler Opened;
        public event EventHandler Closed;

        protected virtual void OnOpened( EventArgs args )
        {
            EventHandler handler = Opened;
            if( handler != null )
            {
                handler( this, args );
            }
        }

        protected virtual void OnClosed( EventArgs args )
        {
            EventHandler handler = Closed;
            if( handler != null )
            {
                handler( this, args );
            }
        }

        public virtual void Configure( XmlElement element )
        {
            Logger.Info( "Configuring..." );

            if( element != null )
            {
                using( var reader = new StringReader( element.OuterXml ) )
                {
                    XmlSerializer serializer = new XmlSerializer( settingsType );
                    settings = (TSettings)serializer.Deserialize( reader );
                }
            }
            else
            {
                settings = new TSettings();
            }

            Logger.Info( "Configured." );
            isConfigured = true;
        }

        protected virtual IDisposable GetStateLock( bool write )
        {
            return new SimpleStateLock( stateSync );
        }

        /// <summary>
        /// Wird aufgerufen, um die Initialisierung und Konfiguration abzuschließen.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Wird geworfen, wenn die Initialisierung bereits durchgeführt wurde.</exception>
        public virtual void Init()
        {
            if( isInitalized )
            {
                throw new InvalidOperationException( "The plugin is already initialized." );
            }
            isInitalized = true;
        }

        /// <summary>
        /// Wird von Open aufgerufen, wenn das Plugin nicht bereits gestartet ist.
        /// </summary>
        /// <remarks>
        /// Diese Funktion wird von Open aufgerufen, um den eigentlichen Start umzusetzen.
        /// Open ruft diese Funktion nur auf, wenn das Plugin vorher gestoppt war.
        /// </remarks>
        protected virtual void BaseOpen()
        {
        }

        /// <summary>
        /// Wird von Close aufgerufen, wenn das Plugin nicht bereits geschlossen ist.
        /// </summary>
        /// <remarks>
        /// Diese Funktion wird von Close aufgerufen, um den eigentlichen Stop umzusetzen.
        /// Close ruft diese Funktion nur auf, wenn das Plugin vorher gestartet war.
        /// </remarks>
        protected virtual void BaseClose()
        {
        }

        /// <summary>
        /// Startet das Plugin.
        /// </summary>
        /// <remarks>
        /// Diese Funktion muss problemlos mehrmals hintereinander aufrufbar sein.
        /// Sie kümmert sich um Logging und auslösen der Events und ruft BaseOpen auf.
        /// Im Normalfall genügt es BaseOpen zu überschreiben, um den Start umzusetzen.
        /// </remarks>
        public virtual void Open()
        {
            using( GetStateLock( true ) )
            {
                if( isOpen )
                {
                    return;
                }

                Logger.Info( "Opening..." );

                try
                {
                    BaseOpen();
                }
                catch( Exception x )
                {
                    Logger.Error( x, "Error while opening." );
                    throw;
                }

                Logger.Info( "Opened." );

                isOpen = true;
            }

            OnOpened( EventArgs.Empty );
        }

        /// <summary>
        /// Stopt das Plugin.
        /// </summary>
        /// <remarks>
        /// Diese Funktion muss problemlos mehrmals hintereinander aufrufbar sein.
        /// Sie kümmert sich um Logging und auslösen der Events und ruft BaseClose auf.
        /// Im Normalfall genügt es BaseClose zu überschreiben, um den Stop umzusetzen.
        /// </remarks>
        public virtual void Close()
        {
            using( GetStateLock( true ) )
            {
                if( !isOpen )
                {
                    return;
                }

                Logger.Info( "Closing..." );

                try
                {
                    BaseClose();
                }
                catch( Exception x )
                {
                    Logger.Error( x, "Error while closing." );
                    throw;
                }

                Logger.Info( "Sucessfully closed." );

                isOpen = false;
            }

            OnClosed( EventArgs.Empty );
        }

        protected virtual void Dispose( bool disposing )
        {
            if( disposing )
            {
                Close();
            }
        }

        #region IDisposable Pattern

        public void Dispose()
        {
            GC.SuppressFinalize( this );
            Dispose( true );
        }

        ~AbstractPlugin()
        {
            Dispose( false );
        }

        #endregion
    }
}
