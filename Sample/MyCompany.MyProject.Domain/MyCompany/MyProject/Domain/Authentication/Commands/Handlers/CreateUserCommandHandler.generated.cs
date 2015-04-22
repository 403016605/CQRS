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

namespace MyCompany.MyProject.Domain.Authentication.Commands.Handlers
{
	[GeneratedCode("CQRS UML Code Generator", "1.500.523.412")]
	public  partial class CreateUserCommandHandler : ICommandHandler<Cqrs.Authentication.ISingleSignOnToken, CreateUserCommand>
	{
		protected IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> UnitOfWork { get; private set; }

		protected IDependencyResolver DependencyResolver { get; private set; }

		protected ILog Log { get; private set; }

		public CreateUserCommandHandler(IUnitOfWork<Cqrs.Authentication.ISingleSignOnToken> unitOfWork, IDependencyResolver dependencyResolver, ILog log)
		{
			UnitOfWork = unitOfWork;
			DependencyResolver = dependencyResolver;
			Log = log;
		}

		#region Implementation of ICommandHandler<in CreateUser>

		public void Handle(CreateUserCommand command)
		{
			User item = null;
			OnCreateUser(command, ref item);
			if (item == null)
			{
				item = new User(DependencyResolver, Log, command.Rsn == Guid.Empty ? Guid.NewGuid() : command.Rsn);
				UnitOfWork.Add(item);
			}
			item.CreateUser(command.Name);
			OnCreatedUser(command, item);
			OnAddToUnitOfWork(command, item);
			UnitOfWork.Add(item);
			OnAddedToUnitOfWork(command, item);
			OnCommit(command, item);
			UnitOfWork.Commit();
			OnCommited(command, item);
		}

		#endregion

		partial void OnCreateUser(CreateUserCommand command, ref User item);

		partial void OnCreatedUser(CreateUserCommand command, User item);

		partial void OnAddToUnitOfWork(CreateUserCommand command, User item);

		partial void OnAddedToUnitOfWork(CreateUserCommand command, User item);

		partial void OnCommit(CreateUserCommand command, User item);

		partial void OnCommited(CreateUserCommand command, User item);
	}
}
