#if NHIBERNATE

using System;
using System.Collections.Generic;
using Hamster.Plugin.Data;
using Hamster.Plugin.NHibernate;
using NHibernate;
using NHibernate.Criterion;

namespace Hamster.Data
{
	public class NHibernateDataStore : AbstractDataStore
	{
		private NHibernatePlugin database;

		public NHibernateDataStore( NHibernatePlugin database )
		{
			this.database = database;
		}

		public override int Store( string target, System.Xml.XmlElement data, string dataType )
		{
			using( ISession session = database.OpenSession() )
			{
				DataStoreItem item = new DataStoreItem();
				item.HandlerName = target;
				item.Data = data;
				item.DataType = dataType;

				session.Save( item );
				session.Flush();

				return item.DataId;
			}
		}

		public override bool SetError( int dataId, string message )
		{
			using( ISession session = database.OpenSession() )
			{
				DataStoreItem item = session.Get<DataStoreItem>( dataId );
				if( item != null )
				{
					item.Error = message;
					item.ProcessTime = DateTime.Now;
					session.Update( item );
					session.Flush();
				}
				return item != null;
			}
		}

		public override bool SetSuccess( int dataId )
		{
			using( ISession session = database.OpenSession() )
			{
				DataStoreItem item = session.Get<DataStoreItem>( dataId );
				if( item != null )
				{
					item.Error = null;
					item.ProcessTime = DateTime.Now;
					session.Update( item );
					session.Flush();
				}
				return item != null;
			}
		}

		public override void Delete( int dataId )
		{
			using( ISession session = database.OpenSession() )
			{
				DataStoreItem item = session.Get<DataStoreItem>( dataId );
				if( item != null )
				{
					session.Delete( item );
					session.Flush();
				}
			}
		}

		public override void Delete( string target, DataStoreItemType type, DateTime maxDate )
		{
			using( ISession session = database.OpenSession() )
			{
				ICriteria crit = session.CreateCriteria( typeof( DataStoreItem ) );
				crit.Add( Expression.Eq( "Target", target ) );
				crit.Add( Expression.Le( "CreationTime", maxDate ) );
				if( (type&DataStoreItemType.Error) == 0 )
				{
					crit.Add( new NullExpression( "Error" ) );
				}
				if( (type&DataStoreItemType.Success) == 0 )
				{
					crit.Add( new OrExpression( new NotNullExpression( "Error" ), new NullExpression( "ProcessTime" ) ) );
				}
				if( (type&DataStoreItemType.Untouched) == 0 )
				{
					crit.Add( new NotNullExpression( "ProcessTime" ) );
				}

				IList<DataStoreItem> list = crit.List<DataStoreItem>();
				foreach( DataStoreItem item in list )
				{
					session.Delete( item );
				}
				session.Flush();
			}
		}

		public override void DeleteAll( DataStoreItemType type, DateTime maxDate )
		{
			using( ISession session = database.OpenSession() )
			{
				ICriteria crit = session.CreateCriteria( typeof( DataStoreItem ) );
				crit.Add( Expression.Le( "CreationTime", maxDate ) );
				if( (type&DataStoreItemType.Error) == 0 )
				{
					crit.Add( new NullExpression( "Error" ) );
				}
				if( (type&DataStoreItemType.Success) == 0 )
				{
					crit.Add( new OrExpression( new NotNullExpression( "Error" ), new NullExpression( "ProcessTime" ) ) );
				}
				if( (type&DataStoreItemType.Untouched) == 0 )
				{
					crit.Add( new NotNullExpression( "ProcessTime" ) );
				}

				IList<DataStoreItem> list = crit.List<DataStoreItem>();
				foreach( DataStoreItem item in list )
				{
					session.Delete( item );
				}
				session.Flush();
			}
		}

		public override DataStoreItem GetItem( int dataId )
		{
			using( ISession session = database.OpenSession() )
			{
				return session.Get<DataStoreItem>( dataId );
			}
		}

		public override DataStoreItem[] GetItems( DataStoreItemType type )
		{
			using( ISession session = database.OpenSession() )
			{
				ICriteria crit = session.CreateCriteria( typeof( DataStoreItem ) );
				if( (type&DataStoreItemType.Error) == 0 )
				{
					crit.Add( new NullExpression( "Error" ) );
				}
				if( (type&DataStoreItemType.Success) == 0 )
				{
					crit.Add( new OrExpression( new NotNullExpression( "Error" ), new NullExpression( "ProcessTime" ) ) );
				}
				if( (type&DataStoreItemType.Untouched) == 0 )
				{
					crit.Add( new NotNullExpression( "ProcessTime" ) );
				}

				IList<DataStoreItem> list = crit.List<DataStoreItem>();
				DataStoreItem[] result = new DataStoreItem[list.Count];
				list.CopyTo( result, 0 );
				return result;
			}
		}

		public override DataStoreItem[] GetItems( string target, DataStoreItemType type )
		{
			using( ISession session = database.OpenSession() )
			{
				ICriteria crit = session.CreateCriteria( typeof( DataStoreItem ) );
				crit.Add( Expression.Eq( "Target", target ) );
				if( (type&DataStoreItemType.Error) == 0 )
				{
					crit.Add( new NullExpression( "Error" ) );
				}
				if( (type&DataStoreItemType.Success) == 0 )
				{
					crit.Add( new OrExpression( new NotNullExpression( "Error" ), new NullExpression( "ProcessTime" ) ) );
				}
				if( (type&DataStoreItemType.Untouched) == 0 )
				{
					crit.Add( new NotNullExpression( "ProcessTime" ) );
				}

				IList<DataStoreItem> list = crit.List<DataStoreItem>();
				DataStoreItem[] result = new DataStoreItem[list.Count];
				list.CopyTo( result, 0 );
				return result;
			}
		}
	}
}

#endif