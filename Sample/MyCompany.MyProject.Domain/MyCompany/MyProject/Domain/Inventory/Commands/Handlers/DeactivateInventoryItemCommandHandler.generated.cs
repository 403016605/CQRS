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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Cqrs.Commands;
using Cqrs.Configuration;
using Cqrs.Domain;
using Cqrs.Logging;

namespace MyCompany.MyProject.Domain.Inventory.Commands.Handlers
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.523.412")]
	public  partial class DeactivateInventoryItemCommandHandler : ICommandHandler<Cqrs.Authentication.ISingleSignOnToken, DeactivateInventoryItemCommand>
	{
		protected IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> UnitOfWork { get; private set; }

		protected IDependencyResolver DependencyResolver { get; private set; }

		protected ILog Log { get; private set; }

		public DeactivateInventoryItemCommandHandler(IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> unitOfWork, IDependencyResolver dependencyResolver, ILog log)
		{
			UnitOfWork = unitOfWork;
			DependencyResolver = dependencyResolver;
			Log = log;
		}


		#region Implementation of ICommandHandler<in DeactivateInventoryItem>

		public void Handle(DeactivateInventoryItemCommand command)
		{
			OnHandle(command);
		}

		#endregion
		partial void OnHandle(DeactivateInventoryItemCommand command);

	}
}
