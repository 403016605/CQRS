﻿#region Copyright
// // -----------------------------------------------------------------------
// // <copyright company="cdmdotnet Limited">
// // 	Copyright cdmdotnet Limited. All rights reserved.
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using cdmdotnet.Logging;
using Cqrs.Configuration;
using Cqrs.Entities;

namespace Cqrs.DataStores
{
	public class SqlDataStore<TData> : IDataStore<TData>
		where TData : Entity
	{
		internal const string SqlDataStoreDbFileOrServerOrConnectionApplicationKey = @"SqlDataStoreDbFileOrServerOrConnection";

		internal const string SqlDataStoreConnectionNameApplicationKey = @"Cqrs.SqlDataStore.ConnectionStringName";

		protected IConfigurationManager ConfigurationManager { get; private set; }

		public SqlDataStore(IConfigurationManager configurationManager, ILogger logger)
		{
			ConfigurationManager = configurationManager;
			Logger = logger;
			// Use a connection string.
			DbDataContext = CreateDbDataContext();

			// Get a typed table to run queries.
			Table = DbDataContext.GetTable<TData>();
		}

		protected DataContext DbDataContext { get; private set; }

		protected Table<TData> Table { get; private set; }

		protected ILogger Logger { get; private set; }

		protected virtual DataContext CreateDbDataContext()
		{
			string connectionStringKey;
			string applicationKey;
			if (!ConfigurationManager.TryGetSetting(SqlDataStoreConnectionNameApplicationKey, out applicationKey) || string.IsNullOrEmpty(applicationKey))
			{
				if (!ConfigurationManager.TryGetSetting(SqlDataStoreDbFileOrServerOrConnectionApplicationKey, out connectionStringKey) || string.IsNullOrEmpty(connectionStringKey))
				{
					throw new NullReferenceException(string.Format("No application setting named '{0}' was found in the configuration file with the name of a connection string to look for.", SqlDataStoreConnectionNameApplicationKey));
				}
			}
			else
			{
				try
				{
					connectionStringKey = System.Configuration.ConfigurationManager.ConnectionStrings[applicationKey].ConnectionString;
				}
				catch (NullReferenceException exception)
				{
					throw new NullReferenceException(string.Format("No connection string setting named '{0}' was found in the configuration file with the SQL Data Store connection string.", applicationKey), exception);
				}
			}
			return new DataContext(connectionStringKey);
		}

		#region Implementation of IEnumerable

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<TData> GetEnumerator()
		{
			return Table.AsQueryable().GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IQueryable

		/// <summary>
		/// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
		/// </returns>
		public Expression Expression
		{
			get { return Table.AsQueryable().Expression; }
		}

		/// <summary>
		/// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
		/// </returns>
		public Type ElementType
		{
			get { return Table.AsQueryable().ElementType; }
		}

		/// <summary>
		/// Gets the query provider that is associated with this data source.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
		/// </returns>
		public IQueryProvider Provider
		{
			get { return Table.AsQueryable().Provider; }
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Table = null;
			DbDataContext.Dispose();
			DbDataContext = null;
		}

		#endregion

		#region Implementation of IDataStore<TData>

		public virtual void Add(TData data)
		{
			Logger.LogDebug("Adding data to the Sql database", "SqlDataStore\\Add");
			try
			{
				DateTime start = DateTime.Now;
				Table.InsertOnSubmit(data);
				DbDataContext.SubmitChanges();
				DateTime end = DateTime.Now;
				Logger.LogDebug(string.Format("Adding data in the Sql database took {0}.", end - start), "SqlDataStore\\Add");
			}
			finally
			{
				Logger.LogDebug("Adding data to the Sql database... Done", "SqlDataStore\\Add");
			}
		}

		public virtual void Add(IEnumerable<TData> data)
		{
			Logger.LogDebug("Adding data collection to the Sql database", "SqlDataStore\\Add\\Collection");
			try
			{
				DateTime start = DateTime.Now;
				Table.InsertAllOnSubmit(data);
				DbDataContext.SubmitChanges();
				DateTime end = DateTime.Now;
				Logger.LogDebug(string.Format("Adding data in the Sql database took {0}.", end - start), "SqlDataStore\\Add\\Collection");
			}
			finally
			{
				Logger.LogDebug("Adding data collection to the Sql database... Done", "SqlDataStore\\Add\\Collection");
			}
		}

		/// <summary>
		/// Will mark the <paramref name="data"/> as logically (or soft) by setting <see cref="Entity.IsLogicallyDeleted"/> to true
		/// </summary>
		public virtual void Remove(TData data)
		{
			Logger.LogDebug("Removing data from the Sql database", "SqlDataStore\\Remove");
			try
			{
				DateTime start = DateTime.Now;
				data.IsLogicallyDeleted = true;
				Update(data);
				DateTime end = DateTime.Now;
				Logger.LogDebug(string.Format("Removing data from the Sql database took {0}.", end - start), "SqlDataStore\\Remove");
			}
			finally
			{
				Logger.LogDebug("Removing data from the Sql database... Done", "SqlDataStore\\Remove");
			}
		}

		public void Destroy(TData data)
		{
			Logger.LogDebug("Removing data from the Sql database", "SqlDataStore\\Destroy");
			try
			{
				DateTime start = DateTime.Now;
				try
				{
					Table.DeleteOnSubmit(data);
				}
				catch (InvalidOperationException exception)
				{
					if (exception.Message != "Cannot remove an entity that has not been attached.")
						throw;
					Table.Attach(data);
					DbDataContext.Refresh(RefreshMode.KeepCurrentValues, data); 
					Table.DeleteOnSubmit(data);
				}
				DbDataContext.SubmitChanges();
				DateTime end = DateTime.Now;
				Logger.LogDebug(string.Format("Removing data from the Sql database took {0}.", end - start), "SqlDataStore\\Destroy");
			}
			finally
			{
				Logger.LogDebug("Removing data from the Sql database... Done", "SqlDataStore\\Destroy");
			}
		}

		public virtual void RemoveAll()
		{
			Logger.LogDebug("Removing all from the Sql database", "SqlDataStore\\RemoveAll");
			try
			{
				Table.Truncate();
			}
			finally
			{
				Logger.LogDebug("Removing all from the Sql database... Done", "SqlDataStore\\RemoveAll");
			}
		}

		public virtual void Update(TData data)
		{
			Logger.LogDebug("Updating data in the Sql database", "SqlDataStore\\Update");
			try
			{
				DateTime start = DateTime.Now;
				Table.Attach(data);
				DbDataContext.Refresh(RefreshMode.KeepCurrentValues, data);
				DbDataContext.SubmitChanges();
				DateTime end = DateTime.Now;
				Logger.LogDebug(string.Format("Updating data in the Sql database took {0}.", end - start), "SqlDataStore\\Update");
			}
			finally
			{
				Logger.LogDebug("Updating data to the Sql database... Done", "SqlDataStore\\Update");
			}
		}

		#endregion
	}
}