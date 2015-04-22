﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#region Copyright
// -----------------------------------------------------------------------
// <copyright company="cdmdotnet Limited">
//     Copyright cdmdotnet Limited. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
#endregion
using System.CodeDom.Compiler;
using Cqrs.DataStores;
using Cqrs.Logging;

namespace MyCompany.MyProject.Domain.Factories
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.523.412")]
	/// <summary>
	/// A factory for obtaining <see cref="IDataStore{TData}"/> instances using an InProcess only approach
	/// </summary>
	public partial class DomainInProcessDataStoreFactory : IDomainDataStoreFactory
	{
		#region Implementation of IDomainDataStoreFactory

		public virtual IDataStore<Inventory.Entities.InventoryItemEntity> GetInventoryItemDataStore()
		{
			IDataStore<Inventory.Entities.InventoryItemEntity> result = null;
			OnGetInventoryItemDataStore(ref result);
			if (result == null)
				result = new InProcessDataStore<Inventory.Entities.InventoryItemEntity>();
			return result;
		}

		public virtual IDataStore<Authentication.Entities.UserEntity> GetUserDataStore()
		{
			IDataStore<Authentication.Entities.UserEntity> result = null;
			OnGetUserDataStore(ref result);
			if (result == null)
				result = new InProcessDataStore<Authentication.Entities.UserEntity>();
			return result;
		}

		#endregion

		partial void OnGetInventoryItemDataStore(ref IDataStore<Inventory.Entities.InventoryItemEntity> result);

		partial void OnGetUserDataStore(ref IDataStore<Authentication.Entities.UserEntity> result);

	}
}
