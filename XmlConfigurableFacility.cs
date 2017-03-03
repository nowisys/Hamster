using System.IO;
using System.Xml;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Hamster.Plugin.Configuration;
using Castle.MicroKernel.Facilities;
using System;

namespace Hamster
{
	public class XmlConfigurableFacility : AbstractFacility
	{
		private string sectionName;
		private string baseDirectory;
		private string pluginManager;

		protected override void Init()
		{
			Kernel.ComponentCreated += ComponentCreated;

			baseDirectory = Path.GetFullPath( FacilityConfig.Attributes["baseDir"] ?? "." );
			sectionName = FacilityConfig.Attributes["sectionName"] ?? "xmlConfig";
			pluginManager = FacilityConfig.Attributes["pluginManager"];
		}

		private IPluginManager GetPluginManager( string name )
		{
			return (IPluginManager)Kernel.Resolve( name, typeof( IPluginManager ) );
		}

		private XmlElement LoadFromFile( string filename, string xpath )
		{
			filename = Path.GetFullPath( Path.Combine( baseDirectory, filename ) );

			if( !File.Exists( filename ) )
			{
				return null;
			}

			XmlDocument doc = new XmlDocument();
			doc.Load( filename );
			XmlElement element;
			if( xpath != null )
			{
				XmlNode node = doc.SelectSingleNode( xpath );
				element = node as XmlElement;
			}
			else
			{
				element = doc.DocumentElement;
			}

			return element;
		}

		private XmlElement LoadFromManager( string name, string pluginName )
		{
			return GetPluginManager( name ).GetPlugin( pluginName ).Settings;
		}

		private XmlElement LoadFromText( string text )
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( text );
			return doc.DocumentElement;
		}

		private void ComponentCreated( ComponentModel model, object instance )
		{
			IXmlConfigurable configurable = instance as IXmlConfigurable;
			if( configurable != null )
			{
				IConfiguration config = model.Configuration == null ? null : model.Configuration.Children[sectionName];
				if( config != null )
				{
					XmlElement settings;
					if( config.Attributes["file"] != null )
					{
						settings = LoadFromFile( config.Attributes["file"], config.Attributes["xpath"] );
					}
					else if( config.Attributes["pluginManager"] != null )
					{
						settings = LoadFromManager( config.Attributes["pluginManager"], model.Name );
					}
					else if( config.Attributes["xml"] != null )
					{
						settings = LoadFromText( config.Attributes["xml"] );
					}
					else
					{
						settings = LoadFromManager( pluginManager, model.Name );
					}

					configurable.Configure( settings );
				}
				else if( pluginManager != null )
				{
					PluginConfig pluginConfig = GetPluginManager( pluginManager ).GetPlugin( model.Name );
					if( pluginConfig != null )
					{
						configurable.Configure( pluginConfig.Settings );
					}
				}

				if( !configurable.IsConfigured )
				{
					configurable.Configure( null );
				}
			}
		}
	}
}
